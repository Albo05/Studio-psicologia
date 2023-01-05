using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azienda_studio_psichiatrico
{
    internal class app : IComparable<app>
    {
        paz _paziente;
        doc _dottore;
        DateTime _data;
        string _arg;
        int pos;

        public paz paziente { get { return _paziente; } set { _paziente = value; } }
        public doc dottore { get { return _dottore; } set { _dottore = value; } }
        public DateTime data { get { return _data; } set { _data = value; } }
        public string argomento { get { return _arg; } set { _arg = value; } }
        public int posizione { get { return pos; } set { pos = value; } }

        public app(paz paziente, doc dottore, DateTime data, string arg, int pos = 0)
        {
            this._paziente = paziente;
            this._dottore = dottore;
            this._data = data;
            this._arg = arg;
            this.pos = pos;
        }



        public int CompareTo(app other)
        {
            int confrontaData = _data.CompareTo(other.data);
            if (confrontaData != 0)
                return confrontaData;
            return _dottore.CompareTo(other.dottore);
        }

        public static app nuovo(string stringa)
        {
            stringa = stringa.Trim();
            string[] appoggioAppuntamento = stringa.Split('*');
            return new app(paz.nuovo(appoggioAppuntamento[0]), doc.nuovo(appoggioAppuntamento[1]), Convert.ToDateTime(appoggioAppuntamento[2]), appoggioAppuntamento[3]);
        }

        public string formattaCombobox
        {
            get { return $"{paziente.nome} {paziente.cognome} → {data.ToString("dd/MM/yyyy HH:mm")}"; }
        }

        public override string ToString()
        {
            return $"{paziente}*{dottore}*{data}*{argomento}".PadRight(60);
        }
    }
}
