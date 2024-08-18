namespace GetNeonIntrinsicsData
{
    /// <summary>
    /// intrinsic 関数の情報を保持するためのクラス
    /// </summary>
    internal class NeonIntrinsic
    {
        public string ReturnType { get; set; }
        public List<Argument> Arguments { get; set; }
        public List<string> InstructionGroups { get; set; }

        /// <summary>
        /// intrinsic のテキスト情報を各パラメータに変換する
        /// </summary>
        /// <param name="retType">戻り値の情報</param>
        /// <param name="args">引数の情報</param>
        /// <param name="instructionGroups">グループの情報</param>
        public NeonIntrinsic(string retType, string args, string instructionGroups)
        {
            this.ReturnType = retType;
            this.Arguments = new List<Argument>();
            this.InstructionGroups = new List<string>();

            string tmpArgs = args.Replace("(", "").Replace(")", "").Trim();
            string[] argArr = tmpArgs.Split(',');
            foreach (string arg in argArr)
            {
                this.Arguments.Add(new Argument(arg.Trim()));
            }

            string[] instructionGroupArr = instructionGroups.Split('/');
            foreach (string instructionGroup in instructionGroupArr)
            {
                this.InstructionGroups.Add(instructionGroup.Trim());
            }
        }
    }

    /// <summary>
    /// intrinsic 関数の引数を保持するためのクラス
    /// </summary>
    public class Argument
    {
        public string DataType { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 引数の情報を型と名前に分ける
        /// </summary>
        /// <param name="src">引数の情報</param>
        public Argument(string src)
        {
            src = src.Trim();
            int lastSpaceIdx = src.LastIndexOf(" ");
            this.DataType = src.Substring(0, lastSpaceIdx);
            this.Name = src.Substring(lastSpaceIdx + 1);
        }
    }
}
