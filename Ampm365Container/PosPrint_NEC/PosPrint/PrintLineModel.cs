using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosPrint
{
    public class PrintLineModel
    {
        /// <summary>
        /// 1、图片；2、条形码；3、文本
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 1、1倍字；2、2倍高1倍宽；3、2倍高2倍宽
        /// </summary>
        public int fontSize { get; set; }
        /// <summary>
        /// 是否加粗 1是0否
        /// </summary>
        public int fontType { get; set; }
        /// <summary>
        /// 换行数
        /// </summary>
        public int splitLine { get; set; }
        /// <summary>
        /// 内容，对应type1、2、3是：url、url、文本
        /// </summary>
        public string content { get; set; }
    }
}
