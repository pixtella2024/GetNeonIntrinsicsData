using OpenQA.Selenium.Chrome;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
using WebDriverManager;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Forms;

namespace GetNeonIntrinsicsData
{
    public partial class MainForm : Form
    {
        private ChromeDriver driver;
        private Dictionary<string, NeonIntrinsic> intrinsicDataDict;
        private static bool IsRunning;
        private static bool IsCanceled;
        private int crawlDelay;

        public MainForm()
        {
            InitializeComponent();
            driver = null;
            intrinsicDataDict = new Dictionary<string, NeonIntrinsic>();
            crawlDelay = 1000;

            txtOutputFileName.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\intrinsic_data_list.json";

            btnCancel.Size = btnExec.Size;
            btnCancel.Location = btnExec.Location;
            btnCancel.Enabled = false;
            btnCancel.Visible = false;
        }

        private async void MainForm_Shown(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "robots_txt_parser.exe 存在確認中";

            toolStripProgressBar1.Maximum = 100;

            Progress<int> progress = new Progress<int>(OnStartupProgressChanged);
            int err = await Task.Run(() => CheckRobotsTxt(progress));

            bool isSuccess = (err == 0);
            if (err == 1)
            {
                MessageBox.Show("robots_txt_parser.exe が見つかりません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (err == 2)
            {
                MessageBox.Show("robots.txt の解析に失敗しました", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (err == 3)
            {
                MessageBox.Show("解析対象サイトへのアクセスに失敗しました", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (err == 4)
            {
                MessageBox.Show("スクレイピングが禁止されました", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            toolStripProgressBar1.Value = 0;

            if (isSuccess)
            {
                toolStripStatusLabel1.Text = "待機中";
                this.Enabled = true;
            }
            else
            {
                toolStripStatusLabel1.Text = "起動失敗";
            }
        }

        private async void btnExec_Click(object sender, EventArgs e)
        {
            btnExec.Enabled = false;
            btnSelectOutputFile.Enabled = false;
            chkShowBrowser.Enabled = false;
            btnCancel.Enabled = true;
            btnCancel.Visible = true;


            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Text = "実行開始";

            if (driver != null)
            {
                driver.Quit();
            }

            intrinsicDataDict.Clear();

            IsCanceled = false;
            IsRunning = true;

            Progress<ProgressVal> progress = new Progress<ProgressVal>(OnProgressChanged);
            int retExec = await Task.Run(() => Exec(progress, txtOutputFileName.Text, chkShowBrowser.Checked));

            IsRunning = false;

            if (retExec == -1)
            {
                toolStripProgressBar1.Value = toolStripProgressBar1.Maximum;
                toolStripStatusLabel1.Text = "中断しました";
            }
            else if (retExec != 0)
            {
                toolStripProgressBar1.Value = toolStripProgressBar1.Maximum;
                toolStripStatusLabel1.Text = "エラー (" + retExec.ToString("X4") + ")";

                if (driver != null)
                {
                    driver.Quit();
                }
            }

            btnCancel.Enabled = false;
            btnCancel.Visible = false;
            btnExec.Enabled = true;
            btnSelectOutputFile.Enabled = true;
            chkShowBrowser.Enabled = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsRunning)
            {
                IsCanceled = true;
                while (IsRunning)
                {
                    Application.DoEvents();
                }
            }

            if (driver != null)
            {
                driver.Quit();
            }
        }

        private void btnSelectOutputFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Title = "出力ファイルを指定してください";
            sfd.InitialDirectory = System.IO.Path.GetDirectoryName(txtOutputFileName.Text);
            sfd.FileName = System.IO.Path.GetFileName(txtOutputFileName.Text);
            sfd.Filter = "JSONファイル(*.json)|*.json";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                txtOutputFileName.Text = sfd.FileName;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            IsCanceled = true;
            btnCancel.Enabled = false;
        }

        /// <summary>
        /// 起動時の robots.txt 確認処理
        /// </summary>
        /// <param name="prog">プログレスバーに渡す値</param>
        /// <returns>エラーコード</returns>
        private int CheckRobotsTxt(IProgress<int> prog)
        {
            int ret = 0;
            if (!System.IO.File.Exists("robots_txt_parser.exe"))
            {
                ret = 1;
            }
            else
            {
                prog.Report(50);

                Process proc = new Process();
                proc.StartInfo.FileName = "robots_txt_parser.exe";
                proc.StartInfo.Arguments = "https://developer.arm.com/architectures/instruction-sets/intrinsics/";
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                bool procFin = proc.WaitForExit(30 * 1000);

                prog.Report(100);

                if (!procFin)
                {
                    ret = 2;
                }
                else if (proc.ExitCode == -1)
                {
                    ret = 3;
                }
                else if (proc.ExitCode == 0)
                {
                    ret = 4;
                }
                else
                {
                    crawlDelay = proc.ExitCode * 1000;
                }
            }

            return ret;
        }

        /// <summary>
        /// スクレイピングを実行する
        /// </summary>
        /// <param name="prog">プログレスバーに渡す値</param>
        /// <param name="outputFileName">出力ファイル名</param>
        /// <param name="showBrowser">ブラウザの表示有無の設定</param>
        /// <returns>エラーコード</returns>
        private int Exec(IProgress<ProgressVal> prog, string outputFileName, bool showBrowser)
        {
            ProgressVal progressVal = new ProgressVal();

            new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);

            var driverVersion = new ChromeConfig().GetMatchingBrowserVersion();
            var driverPath = $"./Chrome/{driverVersion}/X64/";

            ChromeDriverService service = ChromeDriverService.CreateDefaultService(driverPath);
            var options = new ChromeOptions();

#if !DEBUG
            service.HideCommandPromptWindow = true;
#endif
            if (!showBrowser)
            {
                // ヘッドレスモードでは動作しないようなので表示位置で見えないようにする
                options.AddArgument("--window-position=-32000,-32000");
            }

            driver = new ChromeDriver(service, options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            driver.Navigate().GoToUrl(@"https://developer.arm.com/architectures/instruction-sets/intrinsics/");
            List<IWebElement> elementsSimdIsa = driver.FindElements(By.ClassName("SIMD_ISA")).ToList();
            if (elementsSimdIsa.Count != 1)
            {
                return 0x0100;
            }

            IWebElement elementSimdIsa = elementsSimdIsa[0];
            List<IWebElement> elementsCheckbox = elementSimdIsa.FindElements(By.TagName("ads-checkbox")).ToList();

            int neonIdx;
            for (neonIdx = 0; neonIdx < elementsCheckbox.Count; neonIdx++)
            {
                System.Diagnostics.Debug.WriteLine(neonIdx.ToString() + ": " + elementsCheckbox[neonIdx].Text);
                if (elementsCheckbox[neonIdx].Text == "Neon")
                {
                    break;
                }
            }
            System.Diagnostics.Debug.WriteLine(neonIdx.ToString());
            if (neonIdx == elementsCheckbox.Count)
            {
                return 0x0200;
            }
            Thread.Sleep(crawlDelay);

            elementsCheckbox[neonIdx].Click();
            Thread.Sleep(crawlDelay);

            {
                List<IWebElement> elementsAdsPagination = driver.FindElements(By.TagName("ads-pagination")).ToList();
                if (elementsAdsPagination.Count != 1)
                {
                    return 0x0300;
                }
                IWebElement elementAdsPagination = elementsAdsPagination[0];

                ISearchContext shadowroot = elementAdsPagination.GetShadowRoot();

                List<IWebElement> elementsButton = shadowroot.FindElements(By.CssSelector("button[class='c-pagination-item c-pagination-index']")).ToList();
                int pageIdxMax = -1;
                foreach (IWebElement elementButton in elementsButton)
                {
                    int pageIdx = int.Parse(elementButton.Text);
                    if (pageIdxMax < pageIdx)
                    {
                        pageIdxMax = pageIdx;
                    }
                }
                progressVal.pageMax = pageIdxMax;
                prog.Report(progressVal);
            }

            int currPage = 1;
            while (true)
            {
                if (IsCanceled)
                {
                    return -1;
                }

                progressVal.currPage = currPage;
                prog.Report(progressVal);

                List<IWebElement> elementsHtml = driver.FindElements(By.TagName("html")).ToList();
                if (elementsHtml.Count != 1)
                {
                    return 0x0400;
                }
                IWebElement elementTable = elementsHtml[0];
                AnalizeHtml(elementTable.GetAttribute("innerHTML"));

                List<IWebElement> elementsAdsPagination = driver.FindElements(By.TagName("ads-pagination")).ToList();
                if (elementsAdsPagination.Count != 1)
                {
                    return 0x0500;
                }
                IWebElement elementAdsPagination = elementsAdsPagination[0];

                ISearchContext shadowroot = elementAdsPagination.GetShadowRoot();

                List<IWebElement> elementsButtonNext = shadowroot.FindElements(By.CssSelector("button[class='c-pagination-item c-pagination-action c-pagination-action--next']")).ToList();
                System.Diagnostics.Debug.WriteLine(currPage.ToString());
                if (elementsButtonNext.Count != 1)
                {
                    break;
                }
                IWebElement elementButtonNext = elementsButtonNext[0];

                List<IWebElement> elementsFooter = driver.FindElements(By.TagName("ads-footer")).ToList();
                if (elementsFooter.Count != 1)
                {
                    return 0x0600;
                }
                IWebElement elementFooter = elementsFooter[0];
                Actions actions = new Actions(driver);
                actions.MoveToElement(elementFooter);
                actions.Perform();

                elementButtonNext.Click();
                Thread.Sleep(crawlDelay);

                currPage++;
            }
            progressVal.currPage = progressVal.pageMax;
            prog.Report(progressVal);

            intrinsicDataDict.SerializeToFile(outputFileName);

            progressVal.finSave = true;
            prog.Report(progressVal);

            return 0;
        }

        /// <summary>
        /// HTML を解析する
        /// </summary>
        /// <param name="htmlText">解析対象の HTML</param>
        private void AnalizeHtml(string htmlText)
        {
            System.Diagnostics.Debug.WriteLine(htmlText);

            HtmlParser parser = new HtmlParser();
            IHtmlDocument doc = parser.ParseDocument(htmlText);
            var elementTableRow = doc.GetElementsByClassName("c-table-row false undefined false");
            foreach (var element in elementTableRow)
            {
                var elementsTd = element.GetElementsByTagName("td");
                System.Diagnostics.Debug.WriteLine(elementsTd[2].TextContent);
                System.Diagnostics.Debug.WriteLine(elementsTd[3].TextContent);
                System.Diagnostics.Debug.WriteLine(elementsTd[4].TextContent);
                System.Diagnostics.Debug.WriteLine(elementsTd[5].TextContent);

                intrinsicDataDict.Add(elementsTd[3].TextContent, new NeonIntrinsic(
                    elementsTd[2].TextContent,
                    elementsTd[4].TextContent, elementsTd[5].TextContent));
            }
        }

        /// <summary>
        /// プログレスバーの更新を行うイベント関数（起動時）
        /// </summary>
        /// <param name="val">プログレスバーの値</param>
        private void OnStartupProgressChanged(int val)
        {
            toolStripProgressBar1.Value = val;

            if (val == 50)
            {
                toolStripStatusLabel1.Text = "robots.txt 解析中";
            }
            else if (val == 100)
            {
                toolStripStatusLabel1.Text = "robots.txt 解析完了";
            }

        }

        /// <summary>
        /// プログレスバーの更新を行うイベント関数
        /// </summary>
        /// <param name="val">現状の各値</param>
        private void OnProgressChanged(ProgressVal val)
        {
            if (val.finSave)
            {
                toolStripProgressBar1.Value = toolStripProgressBar1.Maximum;
                toolStripStatusLabel1.Text = "完了";
            }
            else if (val.currPage == val.pageMax)
            {
                toolStripProgressBar1.Value = val.currPage;
                toolStripStatusLabel1.Text = "ファイルに書き出し中";
            }
            else
            {
                toolStripProgressBar1.Maximum = val.CalcProgressMaxVal();
                toolStripProgressBar1.Value = val.currPage;
                toolStripStatusLabel1.Text = "解析中 (" + val.currPage.ToString() + "/" + val.pageMax.ToString() + ")";
            }
        }
    }

    public static class JsonExtensions
    {
        /// <summary>
        /// Jsonファイルへの出力
        /// </summary>
        /// <typeparam name="T">データ型</typeparam>
        /// <param name="obj">オブジェクト</param>
        /// <param name="path">ファイルパス</param>
        public static void SerializeToFile<T>(this T obj, string path)
        {
            System.Text.Encoding encoding = new System.Text.UTF8Encoding(false);
            try
            {
                using (var sw = new StreamWriter(path, false, encoding))
                {
                    // JSON データにシリアライズ
                    var jsonData = JsonConvert.SerializeObject(obj, Formatting.Indented);

                    // JSON データをファイルに書き込み
                    sw.Write(jsonData);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"failed:{ex.Message}");
            }
        }
    }

    /// <summary>
    /// プログレスバーに渡す値を保存する構造体
    /// </summary>
    public struct ProgressVal
    {
        public int pageMax;
        public int currPage;
        public bool finSave;

        public ProgressVal()
        {
            this.pageMax = 0;
            this.currPage = 0;
            this.finSave = false;
        }

        public int CalcProgressMaxVal()
        {
            return this.pageMax + (this.pageMax / 10);
        }
    }
}
