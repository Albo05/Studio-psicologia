using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azienda_studio_psichiatrico
{
    class paz : IComparable<paz>
    {
        string _nome;
        string _cognome;
        string _codiceFiscale;

        public string nome { get { return _nome; } set { _nome = value; } }
        public string cognome { get { return _cognome; } set { _cognome = value; } }
        public string codiceFiscale { get { return _codiceFiscale; } set { _codiceFiscale = value; } }

        public paz(string nome, string cognome, string codiceFiscale)
        {
            this._nome = nome;
            this._cognome = cognome;
            this._codiceFiscale = codiceFiscale;
        }

        public bool Uguale(paz verifica)
        {
            if(verifica._codiceFiscale == _codiceFiscale)
                return true;
            return false;
        }

        public override string ToString()
        {
            return $"{nome},{cognome},{codiceFiscale}";
        }

        public int CompareTo(paz other)
        {
            int compareNome = _nome.CompareTo(other.nome);
            if (compareNome != 0)
                return compareNome;
            int compareCognome = _cognome.CompareTo(other.cognome);
            if (compareCognome != 0)
                return compareCognome;
            return _codiceFiscale.CompareTo(other.codiceFiscale);
        }

        public static paz nuovo(string stringa)
        {
            stringa = stringa.Trim();
            string[] appoggioPaziente = stringa.Split(',');
            return new paz(appoggioPaziente[0], appoggioPaziente[1], appoggioPaziente[2]);
        }
    }
}
