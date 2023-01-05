using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace azienda_studio_psichiatrico
{
    public partial class Form1 : Form
    {

        List<doc> dottori = new List<doc>();
        List<paz> pazienti = new List<paz>();
        List<app> appuntamenti = new List<app>();

        public Form1()
        {
            InitializeComponent();
            panel1.Visible = true;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            Aggiungi.Visible = false;
            Calendario.Visible = false;
            Appuntamento.Visible = false;
            if (File.Exists("pazienti.cheSchifoIFileBinari"))
                caricaPazientiOrdinati();
            if(File.Exists("appuntamenti.cheSchifoIFileBinari"))
                caricaAppuntamentiOrdinati();
            if(File.Exists("dottori.cheSchifoIFileBinari"))
                caricaDottoriOrdinati();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // OROLOGIO
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private void orologio_Tick(object sender, EventArgs e)
        {
            string ore = "";
            ore += DateTime.Now.Hour.ToString();
            ore += " : ";
            ore += DateTime.Now.Minute.ToString();
            clockLbl.Text = ore;
            dateLbl.Text = DateTime.Now.ToLongDateString();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // AGGIUNGERE UN APPUNTAMENTO
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void aggiungiAppuntamentoMain()
        {
            if (elementiNonValidiAggiungi())
            {
                MessageBox.Show("Uno o più parametri inseriti sono vuoti");
                return;
            }
            paz pazienteAppuntamentoDaAggiungere = new paz(PazienteNomeAggiungi.Text.ToString(), PazienteCognomeAggiungi.Text.ToString(), PazienteCodiceFiscaleAggiungi.Text.ToString());
            doc dottoreAppuntamentoDaAggiungere = dottori[DottoreSelezioneAggiungi.SelectedIndex];
            if(dottoreLibero(dottoreAppuntamentoDaAggiungere, GiornoSelezioneAggiungi.Value) && pazienteLibero(pazienteAppuntamentoDaAggiungere, GiornoSelezioneAggiungi.Value) && dottoreAppuntamentoDaAggiungere.Assunto)
            {
                SalvaAppuntamenti(new app(pazienteAppuntamentoDaAggiungere, dottoreAppuntamentoDaAggiungere, GiornoSelezioneAggiungi.Value, AppArgomentoAggiungi.Text));
                caricaAppuntamentiOrdinati();
                pazienteEsiste(pazienteAppuntamentoDaAggiungere);
                caricaPazientiOrdinati();
            }
            if (!dottoreAppuntamentoDaAggiungere.Assunto)
                MessageBox.Show("Questo dottore non è assunto in questa clinica");
        }

        private bool dottoreLibero(doc dottore, DateTime giorno)
        {
            TimeSpan inizio = new TimeSpan((int)dottore.OraInizio, (int)(dottore.OraInizio % 1 * 10), 0);
            TimeSpan fine = new TimeSpan((int)dottore.OraFine, (int)(dottore.OraFine % 1 * 10), 0);
            TimeSpan quindici = new TimeSpan(0, 15, 0);
            if (giorno.TimeOfDay > inizio && giorno.TimeOfDay < fine.Subtract(quindici))
            {
                foreach (app a in appuntamenti)
                    if (((a.data < giorno && a.data.Add(quindici) > giorno) || (giorno < a.data && giorno.Add(quindici) > a.data)) && a.dottore.Equals(dottore))
                    {
                        MessageBox.Show("Questo dottore non può ricevere apputamenti in quesrto orario");
                        return false;
                    }
                return true;
            }
            return false;
        }

        private bool pazienteLibero(paz paziente, DateTime giorno)
        {
            TimeSpan quindici = new TimeSpan(0, 15, 0);
            foreach (app a in appuntamenti)
                if (a.data.Date == giorno.Date)
                    if (((a.data < giorno && a.data.Add(quindici) > giorno) || (giorno < a.data && giorno.Add(quindici) > a.data)) && a.paziente.codiceFiscale == paziente.codiceFiscale)
                    {
                        MessageBox.Show("Il paziente ha un appuntamento già fissato in questo intervallo di tempo");
                        return false;
                    }
            return true;
        }

        public double aggiungi15Min(double ore)
        {
            ore += 0.15;
            if(ore%1 >= 0.60)
            {
                ore -= 0.60;
                ore += 1;
                if(ore >= 24)
                    ore -= 24;
            }
            return ore;
        }

        public bool elementiNonValidiAggiungi() 
        { 
            if (PazienteNomeAggiungi.Text.ToString().Trim() == null)
                return true;
            if (PazienteCognomeAggiungi.Text.ToString().Trim() == null)
                return true;
            if (PazienteCodiceFiscaleAggiungi.Text.ToString().Trim() == null)
                return true;
            if (AppArgomentoAggiungi.Text.ToString().Trim() == null)
                return true;
            return false;
        }

        private void pazienteEsiste(paz paziente)
        {
            bool presente = false;
            foreach (paz a in pazienti)
                if(a == paziente)
                    presente = true;
            if (!presente)
                SalvaPazienti(paziente);
        }



        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // AGGIUNGERE UN DOTTORE
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void aggiungiDottoreMain()
        {
            if (elementiNonValidiDottore())
            {
                MessageBox.Show("Uno o più parametri inseriti non sono corretti");
                return;
            }
            doc daAggiungere = new doc(textBox8.Text.ToString(), textBox7.Text.ToString(), dateTimePicker2.Value, Convert.ToDouble(textBox10.Text.ToString()), Convert.ToDouble(textBox9.Text.ToString()), textBox6.Text.ToString(), true);
            if (esiste(daAggiungere))
            {
                MessageBox.Show("Il dottore inserito è già stato assunto all'interno di questo studio");
                return;
            }
            SalvaDottori(daAggiungere);
            caricaDottoriOrdinati();
        }

        private bool esiste(doc dottore)
        {
            bool esiste = false;
            foreach (doc a in dottori)
                if(a.Nome == dottore.Nome && a.Cognome == dottore.Cognome)
                    esiste = true;
            return esiste;
        }

        public bool elementiNonValidiDottore()
        {
            if (dateTimePicker2.Value.Year > DateTime.Now.Year - 18)
                return true;
            if (textBox6.Text.ToString().Trim().Length == 0)
                return true;
            if (textBox9.Text.ToString().Trim().Length == 0 || !double.TryParse(textBox9.Text.ToString(), out double a))
                return true;
            if (textBox10.Text.ToString().Trim().Length == 0 || !double.TryParse(textBox10.Text.ToString(), out a))
                return true;
            if (textBox8.Text.ToString().Trim().Length == 0)
                return true;
            if (textBox7.Text.ToString().Trim().Length == 0)
                return true;
            return false;
        }



        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // CARICARE PAZIENTI
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void caricaPazientiOrdinati()
        {
            pazienti.Clear();
            using (FileStream file = new FileStream("pazienti.cheSchifoIFileBinari", FileMode.Open))
            {
                BinaryReader lettore = new BinaryReader(file);
                do
                {
                    file.Seek(lettore.ReadInt32(), SeekOrigin.Begin);
                    pazienti.Add(paz.nuovo(lettore.ReadString()));
                    int pos = Convert.ToInt32(file.Position);
                    if (lettore.ReadInt32() == 0)
                    {
                        return;
                    }
                    file.Seek(pos, SeekOrigin.Begin);
                } while (1 == 1) ;
            }
        }



        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // SALVARE UN PAZIENTE
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        void SalvaPazienti(paz daSalvare)
        {
            using (FileStream file = new FileStream("pazienti.cheSchifoIFileBinari", FileMode.OpenOrCreate))
            {
                BinaryWriter scrittore = new BinaryWriter(file);
                if (file.Length > 0)
                {
                    BinaryReader lettore = new BinaryReader(file);
                    bool inserito = false;
                    paz prelevato = null;
                    int puntatoreLetto = 0;
                    int byteLetto = 0;
                    do
                    {
                        byteLetto = Convert.ToInt32(file.Position);
                        puntatoreLetto = lettore.ReadInt32();
                        if (puntatoreLetto != 0)
                        {
                            file.Seek(puntatoreLetto, SeekOrigin.Begin);
                            prelevato = paz.nuovo(lettore.ReadString());
                        }
                        if (puntatoreLetto == 0 || prelevato.CompareTo(daSalvare) == -1)
                        {
                            file.Seek(byteLetto, SeekOrigin.Begin);
                            scrittore.Write(Convert.ToInt32(file.Length));
                            file.Seek(0, SeekOrigin.End);
                            scrittore.Write(daSalvare.ToString().PadLeft(500));
                            scrittore.Write(puntatoreLetto);
                            inserito = true;
                        }
                    } while (!inserito);
                }
                else
                {
                    scrittore.Write(4);
                    scrittore.Write(daSalvare.ToString().PadLeft(500));
                    scrittore.Write(0);
                }
            }
        }



        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // CARICARE APPUNTAMENTI
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void caricaAppuntamentiOrdinati()
        {
            appuntamenti.Clear();
            using (FileStream file = new FileStream("appuntamenti.cheSchifoIFileBinari", FileMode.Open))
            {
                BinaryReader lettore = new BinaryReader(file);
                do
                {
                    int posi = lettore.ReadInt32();
                    file.Seek(posi, SeekOrigin.Begin);
                    appuntamenti.Add(app.nuovo(lettore.ReadString()));
                    appuntamenti.Last().posizione = posi;
                    int pos = Convert.ToInt32(file.Position);
                    if (lettore.ReadInt32() == 0)
                    {
                        appuntamenti.Reverse();
                        return;
                    }
                    file.Seek(pos, SeekOrigin.Begin);
                } while (1 == 1);
            }
        }



        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // SALVARE UN APPUNTAMENTO
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        void SalvaAppuntamenti(app daSalvare)
        {
            using (FileStream file = new FileStream("appuntamenti.cheSchifoIFileBinari", FileMode.OpenOrCreate))
            {
                BinaryWriter scrittore = new BinaryWriter(file);
                if (file.Length > 0)
                {
                    BinaryReader lettore = new BinaryReader(file);
                    bool inserito = false;
                    app prelevato = null;
                    int puntatoreLetto = 0;
                    int byteLetto = 0;
                    do
                    {
                        byteLetto = Convert.ToInt32(file.Position);
                        puntatoreLetto = lettore.ReadInt32();
                        if (puntatoreLetto != 0)
                        {
                            file.Seek(puntatoreLetto, SeekOrigin.Begin);
                            prelevato = app.nuovo(lettore.ReadString());
                        }
                        if (puntatoreLetto == 0 || prelevato.CompareTo(daSalvare) == -1)
                        {
                            file.Seek(byteLetto, SeekOrigin.Begin);
                            scrittore.Write(Convert.ToInt32(file.Length));
                            file.Seek(0, SeekOrigin.End);
                            scrittore.Write(daSalvare.ToString().PadLeft(500));
                            scrittore.Write(puntatoreLetto);
                            inserito = true;
                        }
                    } while (!inserito);
                }
                else
                {
                    scrittore.Write(4);
                    scrittore.Write(daSalvare.ToString().PadLeft(500));
                    scrittore.Write(0);
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // CARICARE DOTTORI
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void caricaDottoriOrdinati()
        {
            dottori.Clear();
            using (FileStream file = new FileStream("dottori.cheSchifoIFileBinari", FileMode.Open))
            {
                BinaryReader lettore = new BinaryReader(file);
                do
                {
                    file.Seek(lettore.ReadInt32(), SeekOrigin.Begin);
                    dottori.Add(doc.nuovo(lettore.ReadString()));
                    int pos = Convert.ToInt32(file.Position);
                    if (lettore.ReadInt32() == 0)
                    {
                        return;
                    }
                    file.Seek(pos, SeekOrigin.Begin);
                } while (1 == 1);
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // SALVARE UN DOTTORE
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        void SalvaDottori(doc daSalvare)
        {
            using (FileStream file = new FileStream("dottori.cheSchifoIFileBinari", FileMode.OpenOrCreate))
            {
                BinaryWriter scrittore = new BinaryWriter(file);
                if (file.Length > 0)
                {
                    BinaryReader lettore = new BinaryReader(file);
                    bool inserito = false;
                    doc prelevato = null;
                    int puntatoreLetto = 0;
                    int byteLetto = 0;
                    do
                    {
                        byteLetto = Convert.ToInt32(file.Position);
                        puntatoreLetto = lettore.ReadInt32();
                        if (puntatoreLetto != 0)
                        {
                            file.Seek(puntatoreLetto, SeekOrigin.Begin);
                            prelevato = doc.nuovo(lettore.ReadString());
                        }
                        if (puntatoreLetto == 0 || prelevato.CompareTo(daSalvare) == -1)
                        {
                            file.Seek(byteLetto, SeekOrigin.Begin);
                            scrittore.Write(Convert.ToInt32(file.Length));
                            file.Seek(0, SeekOrigin.End);
                            scrittore.Write(daSalvare.ToString().PadLeft(500));
                            scrittore.Write(puntatoreLetto);
                            inserito = true;
                        }
                    } while (!inserito);
                }
                else
                {
                    scrittore.Write(4);
                    scrittore.Write(daSalvare.ToString().PadLeft(500));
                    scrittore.Write(0);
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // FUNZIONI VARIE DI CARICAMENTO DATI IN OGGETTI
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void caricaDottori(ComboBox cb)
        {
            cb.Items.AddRange(dottori.Where((a)=>a.Assunto).ToArray());
            cb.DisplayMember = "stringaFormattataCombobox";
        }

        public void caricaAppuntamentiData(DateTime dt, Label l)
        {
            List<app> dataGiusta = new List<app>();
            foreach (app a in appuntamenti)
                if(a.data.Date == dt.Date)
                    dataGiusta.Add(a);
            string appuntamentiFormattati = "";
            foreach (app a in dataGiusta)
                appuntamentiFormattati += $"{a.data.ToShortTimeString()} {a.paziente.nome} {a.paziente.cognome}, medico: {a.dottore.Nome} {a.dottore.Cognome} → {a.argomento}\n\n";
            l.Text = appuntamentiFormattati;
        }

        public void caricaAppuntamentiDottore(ComboBox cb, Label l)
        {
            List<app> dottoreGiusto = new List<app>();
            foreach (app a in appuntamenti)
                if (a.dottore.Equals(cb.SelectedItem))
                    dottoreGiusto.Add(a);
            string appuntamentiFormattati = "";
            foreach (app a in dottoreGiusto)
                appuntamentiFormattati += $"{a.data.ToShortTimeString()} {a.paziente.nome} {a.paziente.cognome}, medico: {a.dottore.Nome} {a.dottore.Cognome} → {a.argomento}\n\n";
            l.Text = appuntamentiFormattati;
        }

        public void caricaAppuntamentiTutti(ComboBox cb)
        {
            cb.Items.AddRange(appuntamenti.ToArray());
            cb.DisplayMember = "formattaCombobox";
        }

        public int trovaPosizioneBinariaAppuntamenti(int index)
        {
            using(FileStream fs = new FileStream("appuntamenti.cheSchifoIFileBinari", FileMode.Open))
            {
                BinaryReader br = new BinaryReader(fs);
                int puntatore = 0;
                app appoggio;
                bool trovato = false;
                do
                {
                    puntatore = br.ReadInt32();
                    fs.Seek(puntatore, SeekOrigin.Begin);
                    appoggio = app.nuovo(br.ReadString());
                    if (appoggio == appuntamenti[index])
                        return puntatore; trovato = true;
                } while (!trovato);
                return Convert.ToInt32(fs.Length);
            }
        }

        public int trovaPosizioneBinariaDottori(int index)
        {
            using (FileStream fs = new FileStream("dottori.cheSchifoIFileBinari", FileMode.Open))
            {
                BinaryReader br = new BinaryReader(fs);
                int puntatore = 0;
                doc appoggio;
                bool trovato = false;
                do
                {
                    puntatore = br.ReadInt32();
                    fs.Seek(puntatore, SeekOrigin.Begin);
                    appoggio = doc.nuovo(br.ReadString());
                    if (appoggio.ToString() == dottori[index].ToString())
                        return puntatore;
                } while (!trovato);
                return Convert.ToInt32(fs.Length);
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // BOTTONI  
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private void BottoneSalvaAggiungi_Click(object sender, EventArgs e)
        {
            aggiungiAppuntamentoMain();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Aggiungi.Visible = true;
            Calendario.Visible = false;
            Appuntamento.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel2.Visible = false;
            PazienteNomeAggiungi.Clear();
            PazienteCognomeAggiungi.Clear();
            PazienteCodiceFiscaleAggiungi.Clear();
            DottoreSelezioneAggiungi.SelectedItem = null;
            GiornoSelezionaAggiungi.Refresh();
            caricaDottori(DottoreSelezioneAggiungi);
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            Aggiungi.Visible = false;
            Calendario.Visible = true;
            Appuntamento.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel2.Visible = false;
            GiornoCalendario.Refresh();
            ListaAppuntamentiAggiugni.Text = "";
            caricaDottori(comboBox2);
            comboBox2.SelectedItem = null;
        }

        private void VaiAppuntamenti_Click(object sender, EventArgs e)
        {
            caricaAppuntamentiData(GiornoCalendario.Value, ListaAppuntamentiAggiugni);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            caricaAppuntamentiDottore(comboBox2, ListaAppuntamentiAggiugni);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            Aggiungi.Visible = false;
            Calendario.Visible = false;
            Appuntamento.Visible = true;
            panel3.Visible = false;
            panel4.Visible = false;
            panel2.Visible = false;
            comboBox3.SelectedItem = null;
            PazienteNomeAppuntamento7.Clear();
            PazienteCognomeAppuntamento.Clear();
            PazienteIbanAppuntamento.Clear();
            GiornoSelezionaAppuntamento.Refresh();
            AppArgomentoAppuntamento.Text = null;
            caricaAppuntamentiTutti(comboBox3);
            comboBox3.SelectedItem = null;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            app a = (app)comboBox3.SelectedItem;
            PazienteNomeAppuntamento7.Text = a.paziente.nome.ToString();
            PazienteCognomeAppuntamento.Text = a.paziente.cognome.ToString();
            PazienteIbanAppuntamento.Text = a.paziente.codiceFiscale.ToString();
            caricaDottori(DottoreSelezioneAppuntamento);
            DottoreSelezioneAppuntamento.SelectedItem = a.dottore;
            for (int i = 0; i < dottori.Count; i++)
                if (dottori[i] == a.dottore)
                    DottoreSelezioneAppuntamento.SelectedIndex = i;
            GiornoSelezioneAppuntamento.Value = a.data;
            AppArgomentoAppuntamento.Text = a.argomento.ToString();
            comboBox3.SelectedItem = null;
        }

        private void SalvaAppuntamento_Click(object sender, EventArgs e)
        {
            int dove = trovaPosizioneBinariaAppuntamenti(comboBox3.SelectedIndex);
            paz pazienteAppuntamentoDaModificare = new paz(PazienteNomeAppuntamento7.Text.ToString(), PazienteCognomeAppuntamento.Text.ToString(), PazienteIbanAppuntamento.Text.ToString());
            doc dottoreAppuntamentoDaModificare = dottori[DottoreSelezioneAppuntamento.SelectedIndex];
            app nuovo = new app(pazienteAppuntamentoDaModificare, dottoreAppuntamentoDaModificare, GiornoSelezioneAppuntamento.Value, AppArgomentoAppuntamento.Text);
            using(FileStream fs = new FileStream("appuntamenti.cheSchifoIFileBinari", FileMode.Open))
            {
                BinaryWriter br = new BinaryWriter(fs);
                fs.Seek(((app)comboBox3.SelectedItem).posizione, SeekOrigin.Begin);
                br.Write(nuovo.ToString().PadLeft(500));
            }
            caricaAppuntamentiOrdinati();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Aggiungi.Visible = false;
            Calendario.Visible = false;
            Appuntamento.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel2.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Aggiungi.Visible = false;
            Calendario.Visible = false;
            Appuntamento.Visible = false;
            panel3.Visible = false;
            panel4.Visible = true;
            panel2.Visible = false;
            textBox8.Clear();
            textBox7.Clear();
            dateTimePicker2.Refresh();
            textBox10.Clear();
            textBox6.Clear();
            textBox9.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            aggiungiDottoreMain();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Aggiungi.Visible = false;
            Calendario.Visible = false;
            Appuntamento.Visible = false;
            panel3.Visible = true;
            panel4.Visible = false;
            panel2.Visible = false;
            comboBox1.Items.Clear();
            caricaDottori(comboBox1);
            comboBox1.SelectedItem = null;
            textBox8.Clear();
            textBox3.Clear();
            textBox1.Clear();
            textBox2.Clear();
            textBox5.Clear();
            dateTimePicker1.Refresh();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            doc selezionato = dottori[comboBox1.SelectedIndex];
            textBox4.Text = selezionato.Nome;
            textBox3.Text = selezionato.Cognome;
            dateTimePicker1.Value = selezionato.Nascita;
            textBox1.Text = selezionato.OraInizio.ToString();
            textBox2.Text = selezionato.OraFine.ToString(); 
            textBox5.Text = selezionato.Specializzazione;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            doc daInserire = new doc(textBox4.Text, textBox3.Text, dateTimePicker1.Value, Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox2.Text), textBox5.Text, true);
            int dove = trovaPosizioneBinariaDottori(comboBox1.SelectedIndex);
            using (FileStream fs = new FileStream("dottori.cheSchifoIFileBinari", FileMode.Open))
            {
                BinaryWriter br = new BinaryWriter(fs);
                fs.Seek(dove, SeekOrigin.Begin);
                br.Write(daInserire.ToString().PadLeft(500));
            }
            caricaDottoriOrdinati();
            comboBox1.Items.Clear();
            caricaDottori(comboBox1);
            comboBox1.SelectedIndex = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            doc daInserire = dottori[comboBox1.SelectedIndex];
            int dove = trovaPosizioneBinariaDottori(comboBox1.SelectedIndex);
            using (FileStream fs = new FileStream("dottori.cheSchifoIFileBinari", FileMode.Open))
            {
                daInserire.Assunto = false;
                BinaryWriter br = new BinaryWriter(fs);
                fs.Seek(dove, SeekOrigin.Begin);
                br.Write(daInserire.ToString().PadLeft(500));
            }
            caricaDottoriOrdinati();
            comboBox1.Items.Clear();
            caricaDottori(comboBox1);
            Appuntamento.Visible = true;
            panel3.Visible = false;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
