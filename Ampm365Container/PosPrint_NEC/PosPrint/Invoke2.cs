using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace PosPrint
{
    public class Invoke2
    {
        [DllImport("pos_ad_dll.dll")]
        public static extern int read_standard(StringBuilder buf);
        [DllImport("pos_ad_dll.dll")]
        public static extern int OpenPrinter();
        [DllImport("pos_ad_dll.dll")]
        public static extern int ClosePrinterEx();
        [DllImport("pos_ad_dll.dll")]
        public static extern int BeginPrint(int PrintType);
        //[DllImport("pos_ad_dll.dll")]
        //public static extern int CutPaper(int i);
        [DllImport("pos_ad_dll.dll")]
        public static extern int PrintBitmapFile(StringBuilder fileName, int LabelAngle);
        [DllImport("pos_ad_dll.dll")]
        public static extern int PrintMiddleBitmapFile(StringBuilder fileName, int LabelAngle);
        [DllImport("pos_ad_dll.dll")]
        public static extern int PrintText(StringBuilder content, int FontSize);
        [DllImport("pos_ad_dll.dll")]
        public static extern int PrintTextByPaperWidth(StringBuilder content, int FontSize, int PaperWidth);
    }
}

//#ifndef _POS_AD_DLL_UAN_H
//#define _POS_AD_DLL_UAN_H

//#define HS_OK       0xf0
//#define HS_ERROR    0xff


////extern "C" void End_pos_ad_dll(void);


///*
//    read_baseinfo() buf 数据结构定义
//const struct EBalanceInfo
//{
//    long maxRange[3]; //最大秤量
//    long minRange; //最小秤量
//    long maxTare; //最大皮重
//    long maxPreTare; //最大预置皮重
//    long resolution[3]; //分度
//    char weightUnit[4]; //重量单位
//    char moneyUnit[4]; //货币单位
//    char systemVersion[20]; //数传版本
//} scaleinfo =
//{
//    {6000,15045,0},
//    40,
//    5998,
//    5998,
//    {2,5,0},
//    "Kg",
//    "元",
//    "UAN1.0.0.1"
//};
//*/
//extern "C" int read_baseinfo(char* buf);

///*
//    皮重相关接口 buf数据定义：
//    xy.abcd
//    重量最低位为0.1克。
//*/
//extern "C" int send_tare(char* buf);
//extern "C" int send_pre_tare(char* buf);
//extern "C" int set_tare_bykey(char* buf);
//extern "C" int clear_tare(char* buf);

//extern "C" int send_zero(void);


///*
//    read_standard() buf 中数据定义：
//struct
//{    
//    char status;     
//	//status的bit0(第一位)表示是否稳定，如为1则表示稳定
//	//status的bit1(第二位)表示是否在零位，如为1则表示零位
//	//status的bit2(第三位)表示是否有皮重，如为1则表示有皮重
	
//    char net_weight[7];
//    char FixSeparator;//固定为"P"
//    char tare_weight[7];
//}
//    当处于过载状态时，net_weight中的数据为"┏━┓"
    
//示例：如稳定的净重量为1234g，皮重10g，则返回数据如下
//status的值为5
//net_weight 中的数据为："01.2340"
//FixSeparator的值为字符:'P'
//tare_weight中的数据为："00.0100"

//*/
//extern "C" int read_standard(char* buf);


//extern "C" int OpenCashDrawerEx(void);

//extern "C" int OpenCashDrawerEx_Aux(void);

////custom display routines
//extern "C" int ShowProduct(char* plu_name, int unit_price, int price_type, int count, char* unit, int roundtype);

//extern "C" int ShowDisplayText(int x_pos, int y_pos, int font_size, char* Text);
//extern "C" int ShowBalance(int total_price, int charge, int change);

////printing stuffs
//extern "C" int OpenPrinter(void);
//extern "C" int ClosePrinterEx(void);


///*
//    printer type definitions:
//    8：cut paper
//    other: printing
//*/
//extern "C" int BeginPrint(int PrintType);


///*
//    BmpFileName: 文件路径名，支持UTF8格式
//    LabelAngle:     NA
//*/
//extern "C" int PrintBitmapFile(char* BmpFileName, int LabelAngle);


///*
//    str:        带打印文本，以回车或换行结尾
//    FontSize:   字体大小，以点位单位
//    PaperWidth: NA
//*/
//extern "C" int PrintTextByPaperWidth(char* str, int FontSize, int PaperWidth);
//extern "C" int PrintText(char* str, int FontSize);

//#define MAXDLLPATHLEN   200
//extern "C" int get_uan_configfile_path(char* buf);
//extern "C" int get_uan_logfont_ticket_path(char* buf);
//extern "C" int get_uan_logfont_label_path(char* buf);
//extern "C" int get_uan_logfile_path(char* buf);
//extern "C" int get_uan_bar_dll_path(char* buf);

///*  i:  NA
//*/
//extern "C" int CutPaper(int i);


////stdcall functions
//int __stdcall read_baseinfo_stdcall(char* buf);
//int __stdcall send_zero_stdcall(void);
//int __stdcall send_tare_stdcall(char* buf);

//int __stdcall send_pre_tare_stdcall(char* buf);

//int __stdcall set_tare_bykey_stdcall(char* buf);
//int __stdcall clear_tare_stdcall(char* buf);
//int __stdcall read_standard_stdcall(char* buf);
//int __stdcall OpenCashDrawerEx_stdcall(void);
//int __stdcall OpenCashDrawerEx_Aux_stdcall(void);

////custom display routines
//int __stdcall ShowProduct_stdcall(char* plu_name, int unit_price, int price_type, int count, char* unit, int roundtype);

//int __stdcall ShowDisplayText_stdcall(int x_pos, int y_pos, int font_size, char* Text);

//int __stdcall ShowBalance_stdcall(int total_price, int charge, int change);

////printing stuffs
//int __stdcall OpenPrinter_stdcall(void);
//int __stdcall ClosePrinterEx_stdcall(void);
//int __stdcall PrintTextByPaperWidth_stdcall(char* str, int FontSize, int PaperWidth);
//int __stdcall PrintText_stdcall(char* str, int FontSize);
//int __stdcall BeginPrint_stdcall(int PrintType);
//int __stdcall PrintBitmapFile_stdcall(char* BmpFileName, int LabelAngle);

//#endif
