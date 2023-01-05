using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace azienda_studio_psichiatrico
{
    internal class doc : IComparable<doc>
    {
        string _nome;
        string _cognome;
        DateTime _nascita;
        double _oraInizio;
        double _oraFine;
        string _specializzazione;
        bool _assunto;



        public doc(string nome, string cognome, DateTime nascita, double oraInizio, double oraFine, string specializzazione, bool assunto)
        {
            this.Nome = nome;
            this.Cognome = cognome;
            this.Nascita = nascita;
            this.OraInizio = oraInizio;
            this.OraFine = oraFine;
            this.Specializzazione = specializzazione;
            this.Assunto = assunto;
        }

        public string Nome { get => _nome; set => _nome = value; }
        public string Cognome { get => _cognome; set => _cognome = value; }
        public DateTime Nascita { get => _nascita; set => _nascita = value; }
        public double OraInizio { get => _oraInizio; set => _oraInizio = value; }
        public double OraFine { get => _oraFine; set => _oraFine = value; }
        public string Specializzazione { get => _specializzazione; set => _specializzazione = value; }
        public bool Assunto { get => _assunto; set => _assunto = value; }

        public int CompareTo(doc other)
        {
            int confrontaNome = _nome.CompareTo(other.Nome);
            if (confrontaNome != 0)
                return confrontaNome;
            int confrontaCognome = _cognome.CompareTo(other.Cognome);
            if (confrontaCognome != 0)
                return confrontaCognome;
            int confrontaNascita = _nascita.CompareTo(other.Nascita);
            if (confrontaNascita != 0)
                return confrontaNascita;
            int confrontaSpecializzazione = _specializzazione.CompareTo(other.Specializzazione);
            if(confrontaSpecializzazione != 0)
                return confrontaSpecializzazione;
            return 1;
        }

        public static doc nuovo(string stringa)
        {
            stringa = stringa.Trim();
            string[] appoggioDottore = stringa.Split(',');
            return new doc(appoggioDottore[0], appoggioDottore[1], Convert.ToDateTime(appoggioDottore[2]), Convert.ToDouble(appoggioDottore[3]), Convert.ToDouble(appoggioDottore[4]), appoggioDottore[5], Convert.ToBoolean(appoggioDottore[6]));
        }

        public string stringaFormattataCombobox
        {
            get { return $"{Nome} {Cognome} → {Specializzazione}"; }
        }

        public override string ToString()
        {
            return $"{Nome},{Cognome},{Nascita},{OraInizio},{OraFine},{Specializzazione},{Assunto}";
        }

        public override bool Equals(object o)
        {
            if (!(o is doc))
                return false;
            doc obj = (doc)o;
            return (Nome == obj.Nome && Cognome == obj.Cognome && Nascita == obj.Nascita && Specializzazione == obj.Specializzazione);
        }
    }
}
