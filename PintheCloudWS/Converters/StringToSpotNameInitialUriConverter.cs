using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace PintheCloudWS.Converters
{
    public class StringToSpotNameInitialUriConverter : IValueConverter
    {
        // Instances

        private const string A_URI = "/Assets/pajeon/spot_list/png/spot_label_a.png";
        private const string B_URI = "/Assets/pajeon/spot_list/png/spot_label_b.png";
        private const string C_URI = "/Assets/pajeon/spot_list/png/spot_label_c.png";
        private const string D_URI = "/Assets/pajeon/spot_list/png/spot_label_d.png";
        private const string E_URI = "/Assets/pajeon/spot_list/png/spot_label_e.png";
        private const string F_URI = "/Assets/pajeon/spot_list/png/spot_label_f.png";
        private const string G_URI = "/Assets/pajeon/spot_list/png/spot_label_g.png";
        private const string H_URI = "/Assets/pajeon/spot_list/png/spot_label_h.png";
        private const string I_URI = "/Assets/pajeon/spot_list/png/spot_label_i.png";
        private const string J_URI = "/Assets/pajeon/spot_list/png/spot_label_j.png";
        private const string K_URI = "/Assets/pajeon/spot_list/png/spot_label_k.png";
        private const string L_URI = "/Assets/pajeon/spot_list/png/spot_label_l.png";
        private const string M_URI = "/Assets/pajeon/spot_list/png/spot_label_m.png";
        private const string N_URI = "/Assets/pajeon/spot_list/png/spot_label_n.png";
        private const string O_URI = "/Assets/pajeon/spot_list/png/spot_label_o.png";
        private const string P_URI = "/Assets/pajeon/spot_list/png/spot_label_p.png";
        private const string Q_URI = "/Assets/pajeon/spot_list/png/spot_label_q.png";
        private const string R_URI = "/Assets/pajeon/spot_list/png/spot_label_r.png";
        private const string S_URI = "/Assets/pajeon/spot_list/png/spot_label_s.png";
        private const string T_URI = "/Assets/pajeon/spot_list/png/spot_label_t.png";
        private const string U_URI = "/Assets/pajeon/spot_list/png/spot_label_u.png";
        private const string V_URI = "/Assets/pajeon/spot_list/png/spot_label_v.png";
        private const string W_URI = "/Assets/pajeon/spot_list/png/spot_label_w.png";
        private const string X_URI = "/Assets/pajeon/spot_list/png/spot_label_x.png";
        private const string Y_URI = "/Assets/pajeon/spot_list/png/spot_label_y.png";
        private const string Z_URI = "/Assets/pajeon/spot_list/png/spot_label_z.png";
        private const string SPECIAL_URI = "/Assets/pajeon/spot_list/png/spot_label_special.png";



        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string initial = ((string)value).ToLower();
            if (initial.Equals("a"))
                return A_URI;
            else if (initial.Equals("b"))
                return B_URI;
            else if (initial.Equals("c"))
                return C_URI;
            else if (initial.Equals("d"))
                return D_URI;
            else if (initial.Equals("e"))
                return E_URI;
            else if (initial.Equals("f"))
                return F_URI;
            else if (initial.Equals("g"))
                return G_URI;
            else if (initial.Equals("h"))
                return H_URI;
            else if (initial.Equals("i"))
                return I_URI;
            else if (initial.Equals("j"))
                return J_URI;
            else if (initial.Equals("k"))
                return K_URI;
            else if (initial.Equals("l"))
                return L_URI;
            else if (initial.Equals("m"))
                return M_URI;
            else if (initial.Equals("n"))
                return N_URI;
            else if (initial.Equals("o"))
                return O_URI;
            else if (initial.Equals("p"))
                return P_URI;
            else if (initial.Equals("q"))
                return Q_URI;
            else if (initial.Equals("r"))
                return R_URI;
            else if (initial.Equals("s"))
                return S_URI;
            else if (initial.Equals("t"))
                return T_URI;
            else if (initial.Equals("u"))
                return U_URI;
            else if (initial.Equals("v"))
                return V_URI;
            else if (initial.Equals("w"))
                return W_URI;
            else if (initial.Equals("x"))
                return X_URI;
            else if (initial.Equals("y"))
                return Y_URI;
            else if (initial.Equals("z"))
                return Z_URI;
            else
                return SPECIAL_URI;
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
