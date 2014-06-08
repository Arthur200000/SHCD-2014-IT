namespace Qisi.Editor
{
    using Qisi.Editor.Expression;
    using Qisi.Editor.Properties;
    using Qisi.General;
    using System;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Runtime.InteropServices;
    using System.Text;
	/// <summary>
	/// Common methods, and expression lookup.
	/// </summary>
    internal static class CommonMethods
    {
        internal static int height = 0x23;
        private static MemoryIniFile ini = null;
        internal static PrivateFontCollection pfc = null;
		/// <summary>
		/// Calculates the ascent pixel.
		/// </summary>
		/// <returns>The ascent pixel.</returns>
		/// <param name="font">Font.</param>
        internal static float CalcAscentPixel(Font font)
        {
            FontFamily fontFamily = font.FontFamily;
            int cellAscent = fontFamily.GetCellAscent(font.Style);
            return ((font.Size * cellAscent) / ((float) fontFamily.GetEmHeight(font.Style)));
        }

        internal static structexpression CreateExpr(string type, lineexpression parent, Color color, string content = "", int matrixX = 2, int matrixY = 1)
        {
            switch (type)
            {
                case "分式":
                    return new fenshi(parent, color);

                case "上标":
                    return new shangbiao(parent, color);

                case "下标":
                    return new xiabiao(parent, color);

                case "斜分式":
                    return new xiefenshi(parent, color);

                case "上下标右":
                    return new shangxiabiaoyou(parent, color);

                case "上下标左":
                    return new shangxiabiaozuo(parent, color);

                case "平方根":
                    return new pingfanggen(parent, color);

                case "立方根":
                    return new lifanggen(parent, color);

                case "一般根式":
                    return new genshi(parent, color);

                case "积分1":
                    return new jifen1(parent, color);

                case "积分2":
                    return new jifen2(parent, color);

                case "积分3":
                    return new jifen3(parent, color);

                case "围道积分1":
                    return new weidao1(parent, color);

                case "围道积分2":
                    return new weidao2(parent, color);

                case "围道积分3":
                    return new weidao3(parent, color);

                case "面积分1":
                    return new mianjifen1(parent, color);

                case "面积分2":
                    return new mianjifen2(parent, color);

                case "面积分3":
                    return new mianjifen3(parent, color);

                case "体积分1":
                    return new tijifen1(parent, color);

                case "体积分2":
                    return new tijifen2(parent, color);

                case "体积分3":
                    return new tijifen3(parent, color);

                case "二重积分1":
                    return new erchongjifen1(parent, color);

                case "二重积分2":
                    return new erchongjifen2(parent, color);

                case "二重积分3":
                    return new erchongjifen3(parent, color);

                case "三重积分1":
                    return new sanchongjifen1(parent, color);

                case "三重积分2":
                    return new sanchongjifen2(parent, color);

                case "三重积分3":
                    return new sanchongjifen3(parent, color);

                case "求和1":
                    return new qiuhe1(parent, color);

                case "求和2":
                    return new qiuhe2(parent, color);

                case "求和3":
                    return new qiuhe3(parent, color);

                case "求和4":
                    return new qiuhe4(parent, color);

                case "求和5":
                    return new qiuhe5(parent, color);

                case "乘积1":
                    return new chengji1(parent, color);

                case "乘积2":
                    return new chengji2(parent, color);

                case "乘积3":
                    return new chengji3(parent, color);

                case "乘积4":
                    return new chengji4(parent, color);

                case "乘积5":
                    return new chengji5(parent, color);

                case "副积1":
                    return new fuji1(parent, color);

                case "副积2":
                    return new fuji2(parent, color);

                case "副积3":
                    return new fuji3(parent, color);

                case "副积4":
                    return new fuji4(parent, color);

                case "副积5":
                    return new fuji5(parent, color);

                case "并11":
                    return new bing11(parent, color);

                case "并12":
                    return new bing12(parent, color);

                case "并13":
                    return new bing13(parent, color);

                case "并14":
                    return new bing14(parent, color);

                case "并15":
                    return new bing15(parent, color);

                case "交11":
                    return new jiao11(parent, color);

                case "交12":
                    return new jiao12(parent, color);

                case "交13":
                    return new jiao13(parent, color);

                case "交14":
                    return new jiao14(parent, color);

                case "交15":
                    return new jiao15(parent, color);

                case "并21":
                    return new bing21(parent, color);

                case "并22":
                    return new bing22(parent, color);

                case "并23":
                    return new bing23(parent, color);

                case "并24":
                    return new bing24(parent, color);

                case "并25":
                    return new bing25(parent, color);

                case "交21":
                    return new jiao21(parent, color);

                case "交22":
                    return new jiao22(parent, color);

                case "交23":
                    return new jiao23(parent, color);

                case "交24":
                    return new jiao24(parent, color);

                case "交25":
                    return new jiao25(parent, color);

                case "正弦":
                    return new sin(parent, color);

                case "余弦":
                    return new cos(parent, color);

                case "正切":
                    return new tan(parent, color);

                case "余切":
                    return new cot(parent, color);

                case "反正弦":
                    return new arcsin(parent, color);

                case "反余弦":
                    return new arccos(parent, color);

                case "反正切":
                    return new arctan(parent, color);

                case "反余切":
                    return new arccot(parent, color);

                case "正割":
                    return new sec(parent, color);

                case "余割":
                    return new csc(parent, color);

                case "双曲正弦":
                    return new sinh(parent, color);

                case "双曲余弦":
                    return new cosh(parent, color);

                case "双曲正切":
                    return new tanh(parent, color);

                case "双曲余切":
                    return new coth(parent, color);

                case "双曲正割":
                    return new sech(parent, color);

                case "双曲余割":
                    return new csch(parent, color);

                case "对数2":
                    return new log(parent, color);

                case "对数e":
                    return new ln(parent, color);

                case "对数10":
                    return new lg(parent, color);

                case "对数":
                    return new logx(parent, color);

                case "正弦_2":
                    return new sin_2(parent, color);

                case "余弦_2":
                    return new cos_2(parent, color);

                case "正切_2":
                    return new tan_2(parent, color);

                case "余切_2":
                    return new cot_2(parent, color);

                case "反正弦_2":
                    return new arcsin_2(parent, color);

                case "反余弦_2":
                    return new arccos_2(parent, color);

                case "反正切_2":
                    return new arctan_2(parent, color);

                case "反余切_2":
                    return new arccot_2(parent, color);

                case "正割_2":
                    return new sec_2(parent, color);

                case "余割_2":
                    return new csc_2(parent, color);

                case "双曲正弦_2":
                    return new sinh_2(parent, color);

                case "双曲余弦_2":
                    return new cosh_2(parent, color);

                case "双曲正切_2":
                    return new tanh_2(parent, color);

                case "双曲余切_2":
                    return new coth_2(parent, color);

                case "双曲正割_2":
                    return new sech_2(parent, color);

                case "双曲余割_2":
                    return new csch_2(parent, color);

                case "极限":
                    return new lim(parent, color);

                case "最大值":
                    return new max(parent, color);

                case "最小值":
                    return new min(parent, color);

                case "点":
                    return new dian(parent, color);

                case "双点":
                    return new shuangdian(parent, color);

                case "三点":
                    return new sandian(parent, color);

                case "乘幂号":
                    return new chengmihao(parent, color);

                case "对号":
                    return new duihao(parent, color);

                case "尖音符号":
                    return new jianyinfuhao(parent, color);

                case "抑音符号":
                    return new yiyinfuhao(parent, color);

                case "短音符号":
                    return new duanyinfuhao(parent, color);

                case "颚化符":
                    return new ehuafu(parent, color);

                case "横杠":
                    return new henggang(parent, color);

                case "双顶线":
                    return new shuangdingxian(parent, color);

                case "底线":
                    return new dixian(parent, color);

                case "带框公式":
                    return new daikuanggongshi(parent, color);

                case "上方大括号":
                    return new shangfangdakuohao(parent, color);

                case "下方大括号":
                    return new xiafangdakuohao(parent, color);

                case "分组字符在上":
                    return new fenzuzifuzaishang(parent, color);

                case "分组字符在下":
                    return new fenzuzifuzaixia(parent, color);

                case "左箭头在上":
                    return new zuojiantouzaishang(parent, color);

                case "右箭头在上":
                    return new youjiantouzaishang(parent, color);

                case "双向箭头在上":
                    return new shuangxiangjiantouzaishang(parent, color);

                case "左向简式箭头在上":
                    return new zuoxiangjianshijiantouzaishang(parent, color);

                case "右向简式箭头在上":
                    return new youxiangjianshijiantouzaishang(parent, color);

                case "大括号":
                    return new dakuohao(parent, color);

                case "方括号":
                    return new fangkuohao(parent, color);

                case "括号":
                    return new kuohao(parent, color);

                case "尖括号":
                    return new jiankuohao(parent, color);

                case "单边大括号":
                    return new danbiandakuohao(parent, color);

                case "绝对值1":
                    return new jueduizhi1(parent, color);

                case "绝对值2":
                    return new jueduizhi2(parent, color);

                case "等号1":
                    return new denghao1(parent, color);

                case "左箭头1":
                    return new zuojiantou1(parent, color);

                case "右箭头1":
                    return new youjiantou1(parent, color);

                case "双向箭头1":
                    return new shuangxiangjiantou1(parent, color);

                case "等号2":
                    return new denghao2(parent, color);

                case "左箭头2":
                    return new zuojiantou2(parent, color);

                case "右箭头2":
                    return new youjiantou2(parent, color);

                case "双向箭头2":
                    return new shuangxiangjiantou2(parent, color);

                case "等号3":
                    return new denghao3(parent, color);

                case "左箭头3":
                    return new zuojiantou3(parent, color);

                case "右箭头3":
                    return new youjiantou3(parent, color);

                case "双向箭头3":
                    return new shuangxiangjiantou3(parent, color);

                case "矩阵":
                    return new juzheng(new Point(matrixX, matrixY), parent, color);

                case "加减号":
                case "无穷":
                case "不等于":
                case "乘号":
                case "三角":
                case "除号":
                case "成比例":
                case "远小于":
                case "远大于":
                case "小于等于":
                case "大于等于":
                case "减加号":
                case "约等于":
                case "几乎等于":
                case "等价于":
                case "任意":
                case "补集":
                case "偏微分":
                case "平方根号":
                case "立方根号":
                case "四方根号":
                case "并":
                case "交":
                case "空集":
                case "度":
                case "华氏度":
                case "摄氏度":
                case "燃烧":
                case "差值":
                case "递增":
                case "递减":
                case "存在":
                case "不存在":
                case "属于于":
                case "属于":
                case "不属于":
                case "包含":
                case "包含于":
                case "不包含1":
                case "不包含于1":
                case "不包含2":
                case "不包含于2":
                case "左箭头":
                case "上箭头":
                case "下箭头":
                case "右箭头":
                case "双向箭头":
                case "因为":
                case "所以":
                case "未签名":
                case "希腊1":
                case "希腊2":
                case "希腊3":
                case "希腊4":
                case "希腊5":
                case "希腊6":
                case "希腊7":
                case "希腊8":
                case "希腊9":
                case "希腊10":
                case "希腊11":
                case "希腊12":
                case "希腊13":
                case "希腊14":
                case "希腊15":
                case "希腊16":
                case "希腊17":
                case "星乘":
                case "点乘":
                case "着重点":
                case "垂直省略号":
                case "中间水平省略号":
                case "右上对角省略号":
                case "右下对角省略号":
                case "希腊18":
                case "希腊19":
                case "直角":
                case "角度":
                case "测量角":
                case "球面角":
                case "以弧度表示的直角":
                case "直角三角形":
                case "等于且平行于":
                case "垂直于":
                case "不整除":
                case "不平行于":
                case "平行于":
                case "比率":
                case "比例":
                case "左双线箭头":
                case "右双线箭头":
                case "左右双线箭头":
                    return new specialchar(GetSpecialChar(type), (FunctionType) Enum.Parse(typeof(FunctionType), type, true), ExprToString(type), parent, color);
            }
            return new charexpression(content, parent, color);
        }
		/// <summary>
		/// Finds the Expression called Key.
		/// </summary>
		/// <param name="key">Key.</param>
        internal static string Exprs(string key)
        {
            if (ini == null)
            {
                ini = new MemoryIniFile();
                ini.LoadFromString(Resources.Expr, Encoding.UTF8);
            }
            return ini.ReadValue("Group", key, "");
        }
		/// <summary>
		/// Expression to string.
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="key">Key.</param>
        internal static string ExprToString(string key)
        {
            if (ini == null)
            {
                ini = new MemoryIniFile();
                ini.LoadFromString(Resources.Expr, Encoding.UTF8);
            }
            return ini.ReadValue("Expr", key, "");
        }
		/// <summary>
		/// Gets the cambria font.
		/// </summary>
		/// <returns>The cambria font.</returns>
		/// <param name="fontsize">Fontsize.</param>
		/// <param name="fontstyle">Fontstyle.</param>
        public static Font GetCambriaFont(float fontsize = 12f, FontStyle fontstyle = 0)
        {
            if (pfc == null)
            {
                pfc = new PrivateFontCollection();
                byte[] cambria = Resources.cambria;
                IntPtr destination = Marshal.AllocCoTaskMem(cambria.Length);
                Marshal.Copy(cambria, 0, destination, cambria.Length);
                pfc.AddMemoryFont(destination, cambria.Length);
                Marshal.FreeCoTaskMem(destination);
                cambria = Resources.cambriab;
                destination = Marshal.AllocCoTaskMem(cambria.Length);
                Marshal.Copy(cambria, 0, destination, cambria.Length);
                pfc.AddMemoryFont(destination, cambria.Length);
                Marshal.FreeCoTaskMem(destination);
                cambria = Resources.cambriaz;
                destination = Marshal.AllocCoTaskMem(cambria.Length);
                Marshal.Copy(cambria, 0, destination, cambria.Length);
                pfc.AddMemoryFont(destination, cambria.Length);
                Marshal.FreeCoTaskMem(destination);
                cambria = Resources.cambriai;
                destination = Marshal.AllocCoTaskMem(cambria.Length);
                Marshal.Copy(cambria, 0, destination, cambria.Length);
                pfc.AddMemoryFont(destination, cambria.Length);
                Marshal.FreeCoTaskMem(destination);
            }
            if (pfc != null)
            {
                for (int i = 0; i < pfc.Families.Length; i++)
                {
                    if (pfc.Families[i].Name == "Cambria")
                    {
                        return new Font(pfc.Families[i], fontsize, fontstyle, GraphicsUnit.Pixel);
                    }
                }
            }
            return SystemFonts.DefaultFont;
        }

        internal static string GetSpecialChar(string key)
        {
            if (ini == null)
            {
                ini = new MemoryIniFile();
                ini.LoadFromString(Resources.Expr, Encoding.UTF8);
            }
            return ini.ReadValue("SpecialChar", key, "");
        }

        internal static string Groups(string key)
        {
            if (ini == null)
            {
                ini = new MemoryIniFile();
                ini.LoadFromString(Resources.Expr, Encoding.UTF8);
            }
            return ini.ReadValue("Subject", key, "");
        }
    }
}

