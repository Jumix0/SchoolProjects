using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace ConsoleApp_Corsa_Net
{
    internal class Program
    {
        static int posAndrea = 0;
        static int posBaldo = 0;
        static int posCarlo = 0;
        static int classifica = 0;
        static Object _lock = new Object();
        static Thread thAndrea;
        static Thread thBaldo; // Creiamo un thread per ogni metodo
        static Thread thCarlo ;
        static string commando = ""; // Tenere la traccia del commando
        static char nome;
        static bool check_join;
 
   

        class Dati_Giocatori // Serve a contere i dati di giocatori nel metodo Giocatori
        {
            int riga;
            string gambe;
            string torso;
            string testa;
            public Dati_Giocatori(int riga, string gambe, string torso, string testa)
            {
                this.riga = riga;
                this.gambe = gambe;
                this.torso = torso;
                this.testa = testa;
            }
            public int Riga {get { return riga; } set { riga = value; } }
            public string Gambe { get {  return gambe; } }
            public string Torso { get {  return torso; } }
            public string Testa { get {  return testa; } }
        }
        static void Giocatori(object obj) // Il metodo che accetta l'object del giocatore, fa partire questo giocatore, 
        {                                 // sostituisce i nostri metodi precedenti: Carlo(), Andrea(), Baldo()
            Dati_Giocatori data = (Dati_Giocatori)obj;
            for (int i = 0; i < 114; i++)
            {
                if (commando.Length == 2) // Verifica se la lunghezza della stringa commando è 2, 
                {                         // se lo è significa che dobbiamo fare thread.Join();
                    if (commando[0] == 'C' && Thread.CurrentThread.Name == "Carlo") // Decodifica la stringa dove si contengono i nomi che,
                        if (commando[1] == 'A')                                     // interaggiscono in join. Prima lettera --> giocatore che va in Join,
                            thAndrea.Join();                                        // seconda lettera --> è il giocatore che deve arrivare in fondo per,
                        else if (commando[1] == 'B')                                // fare ripartire il primo giocatore.
                            thBaldo.Join();
                    if (commando[0] == 'A' && Thread.CurrentThread.Name == "Andrea")
                        if (commando[1] == 'B')
                            thBaldo.Join();
                        else if (commando[1] == 'C')
                            thCarlo.Join();
                    if (commando[0] == 'B' && Thread.CurrentThread.Name == "Baldo")
                        if (commando[1] == 'A')
                            thAndrea.Join();
                        else if (commando[1] == 'C')
                            thCarlo.Join();
                }
                Scrivi(i, data.Riga+2, data.Gambe); // Stampa ogni volta giocatore.
                Thread.Sleep(50);
                Scrivi(i, data.Riga + 1, data.Torso);
                Thread.Sleep(50);
                Scrivi(i, data.Riga, data.Testa);
            }
            lock (_lock) // A quel punto i giocatori arrivano in fondo ed a loro viene assegnata la loro classifica. In base all'arrivazione.
            {
                classifica++;
                SetCursorPosition(115, data.Riga-1);
                if (classifica == 1) { Console.ForegroundColor = ConsoleColor.Green; }
                else if (classifica == 2) { Console.ForegroundColor = ConsoleColor.Yellow; }
                else if (classifica == 3) { Console.ForegroundColor = ConsoleColor.Red; }
                Write(classifica);
                Console.ForegroundColor= ConsoleColor.White;
            }
        }

        /*static void Andrea()
        {
            for (posAndrea = 0; posAndrea < 114; posAndrea++)
            {
                if (commando.Length == 2)
                {
                    if (commando[0] == 'A')
                    {
                        if (commando[1] == 'B')
                            thBaldo.Join();
                        else if (commando[1] == 'C')
                            thCarlo.Join();
                    }
                }
                Thread.Sleep(50);
                lock (_lock)
                {
                    SetCursorPosition(posAndrea, 5);
                    Write(@"  /\");
                }
                Thread.Sleep(50); // Ferma l'elaborazione per 0.50ms
                lock (_lock)
                {
                    SetCursorPosition(posAndrea, 4); ;
                    Write(@" /▒\");
                }
                Thread.Sleep(50);
                lock (_lock)
                {
                    SetCursorPosition(posAndrea, 3);
                    Write("  []");
                }

            }
            lock(_lock)
            {
                classifica++;
                SetCursorPosition(115, 2);
                Write(classifica);
            }
        }
        static void Baldo()
        {
            
                for (posBaldo = 0; posBaldo < 114; posBaldo++)
                {
                    if (commando.Length == 2)
                    {
                        if (commando[0] == 'B')
                        {
                            if (commando[1] == 'A')
                                thAndrea.Join();
                            else if (commando[1] == 'C')
                                thCarlo.Join();
                        }
                    }

                Thread.Sleep(50);
                lock (_lock)
                {
                    SetCursorPosition(posBaldo, 9);
                    Write(@"  ┘└");
                }
                Thread.Sleep(50); // Ferma l'elaborazione per 0.50ms
                lock (_lock)
                {
                    SetCursorPosition(posBaldo, 8); ;
                    Write(@" ┌▒▒┐");
                }
                Thread.Sleep(50);
                lock (_lock)
                {
                    SetCursorPosition(posBaldo, 7);
                    Write("  ()");
                }

                }
                lock (_lock)
                {
                    classifica++;
                    SetCursorPosition(115, 6);
                    Write(classifica);
                    
                }
                
            
        }

        static void Carlo()
        {
            for (posCarlo = 0; posCarlo < 114; posCarlo++)
            {
                if (commando.Length == 2)
                {
                    if (commando[0] == 'C')
                    {
                        if (commando[1] == 'A')
                            thAndrea.Join();
                        else if (commando[1] == 'B')
                            thBaldo.Join();
                    }
                }

                Thread.Sleep(50);
                lock (_lock)
                {
                    SetCursorPosition(posCarlo, 13);
                    Write(@"  ╝╚");

                }
                Thread.Sleep(50); // Ferma l'elaborazione per 0.50ms
                lock (_lock)
                {
                    SetCursorPosition(posCarlo, 12); ;
                    Write(@" /██\");

                }
                Thread.Sleep(50);
                lock (_lock)
                {
                    SetCursorPosition(posCarlo, 11);
                    Write("  <>");
                }

            }
            lock (_lock)
            {
                classifica++;
                SetCursorPosition(115, 10);
                Write(classifica);
            }
            
            
        }*/ // Viene sostituito, con la versione 2 con la class Dati_Giocatori e Giocatori()

        static void Pronti() // Crea i giocatori (caratteri)
        {
            SetCursorPosition(posAndrea, 2);
            //Write("Andrea");
            SetCursorPosition(posAndrea, 3);
            Write("  []");
            SetCursorPosition(posAndrea, 4);
            Write(@"   /█\");
            SetCursorPosition(posAndrea, 5);
            Write(@"  /\");
            SetCursorPosition(posBaldo, 6);
            //Write("Baldo");
            SetCursorPosition(posBaldo, 7);
            Write("  ()");
            SetCursorPosition(posBaldo, 8);
            Write(@" ┌▒▒┐");
            SetCursorPosition(posBaldo, 9);
            Write("  ┘└");
            SetCursorPosition(posCarlo, 10);
            //Write("Carlo");
            SetCursorPosition(posCarlo, 11);
            Write("  <>");
            SetCursorPosition(posCarlo, 12);
            Write(@" /██\");
            SetCursorPosition(posCarlo, 13);
            Write("  ╝╚");
            Console.ForegroundColor = ConsoleColor.White;
        }


        static Thread CheckedJoin()
        {
            Scrivi(0, 15, "              ARRIVO DI CHI DEVO ASPETTARE:               ");
            Scrivi(30, 16, "                      ");
            Scrivi(30, 17, "    A) Andrea             ");
            Scrivi(30, 18, "    B) Baldo              ");
            Scrivi(30, 19, "    C) Carlo              ");
            Scrivi(30, 20, "      ");
            SetCursorPosition(31, 21);
            nome = ReadKey().KeyChar;
            switch (nome)
            {
                case 'A':
                    return thAndrea;

                case 'B':
                    return thBaldo;

                case 'C':
                    return thCarlo;

                default: return null;
            }

        }

        static Thread Persona() // Menu su chi verrà applicato il commando
        {
           
            Scrivi(0, 15,   "                SCEGLIERE PERSONA:                 ");
                Scrivi(30, 16, "                      ");
            Scrivi(30, 17, "    A) Andrea             ");
            Scrivi(30, 18, "    B) Baldo              ");
            Scrivi(30, 19, "    C) Carlo              ");
            Scrivi(30, 20, "      ");
            SetCursorPosition(31, 21);
            nome = ReadKey().KeyChar;
            switch (nome)
            {
                case 'A':
                    return thAndrea;

                case 'B':
                    return thBaldo;

                case 'C':
                    return thCarlo;

                default: return null;
            }
        }
        static bool Menu()// Menu dei commandi
        {
            char commando_switch = ReadKey().KeyChar;
            switch (commando_switch)
            {
                case 'A': // fa il thread di giocatore.Abort();
                    {
                        Persona().Abort();
                       
                    }
                    break;

                case 'S':  // fa il thread di giocatore.Suspend();
                    {
                        var controllo_persona = Persona();
                        if (controllo_persona.ThreadState != ThreadState.Suspended)
                        {
                            if (controllo_persona.IsAlive)
                                controllo_persona.Suspend();
                        }
                    }
                    break;

                case 'R': // fa il thread di giocatore.Resume();
                    {
                       
                        var controllo_persona = Persona();
                        if (controllo_persona.ThreadState == ThreadState.Suspended)
                        {
                            if (controllo_persona.IsAlive)
                                controllo_persona.Resume();

                        }
                    }
                    break;

                case 'J': // fa il join del thread che scegliamo, prima chi andrà in Join, il secondo è chi sarà aspettato dal primo
                    {
                        var controllo_persona = Persona();
                        if (controllo_persona == thAndrea)
                            commando = "A";
                        else if (controllo_persona == thBaldo)
                            commando ="B";
                        else
                            commando ="C";
                        check_join = true;
                        if (check_join == true)
                        {
                            controllo_persona = CheckedJoin();
                            if (controllo_persona == thAndrea)
                                commando += "A";
                            else if (controllo_persona == thBaldo)
                                commando += "B";
                            else
                                commando += "C";
                        }
                        else
                        {
                            controllo_persona = Persona();
                            if (controllo_persona == thAndrea)
                                commando += "A";
                            else if (controllo_persona == thBaldo)
                                commando += "B";
                            else
                                commando += "C";
                        }
                    }
                    break;
            }



            return false;

        }
        static void Scrivi(int col, int rig, string st)
        {
            lock (_lock)
            {

                SetCursorPosition(col, rig);
                Write(@st);
            }
        }
        static void Stato() // Scrive i stati dei thread e se sono vivi.
        {

            
                Scrivi(1, 2, $"Andrea ->          {thAndrea.ThreadState}      ");
                if (thAndrea.IsAlive) Scrivi(40, 2, $"Is alive =   True   ");
                else Scrivi(40, 2, $"Is alive =   False   ");
                Scrivi(1, 6, $"Baldo ->           {thBaldo.ThreadState}       ");
                if (thBaldo.IsAlive) Scrivi(40, 6, $"Is alive =   True   ");
                else Scrivi(40, 6, $"Is alive =   False   ");
                Scrivi(1, 10, $"Carlo ->          {thCarlo.ThreadState}       ");
                if (thCarlo.IsAlive) Scrivi(40, 10, $"Is alive =   True   ");
                else Scrivi(40, 10, $"Is alive =   False   ");
            
                

        }
        
        static void Main(string[] args)
        {
            Console.Title = "Chabaniuk Andrii || 4H || 28.09.2023 || Corsa_Amici_Versione2";
            Console.WriteLine("Simulazione della corsa di tre amici");
            Write("");
            CursorVisible = false; // Elimina il cursore
            SetCursorPosition(WindowWidth / 2 - 26, WindowHeight / 2);
            Write("PREMERE QUALSIASI TASTO PER INIZIARE");
            ReadKey();
            SetCursorPosition(WindowWidth / 2 - 26, WindowHeight / 2);
            Write("                                    ");
            Pronti(); // Creazione dei amici (caratteri))
            thAndrea = new Thread(Giocatori);
            thAndrea.Name = "Andrea";
            thBaldo = new Thread(Giocatori);
            thBaldo.Name = "Baldo";
            thCarlo = new Thread(Giocatori);
            thCarlo.Name = "Carlo";
            Dati_Giocatori andreaData = new Dati_Giocatori(3, @"   ┘└", @"   /█\", @"    []");
            Dati_Giocatori baldoData = new Dati_Giocatori(7, @"   /\", @"  ┌░░┐", @"   !_!");
            Dati_Giocatori carloData = new Dati_Giocatori(11, @"   ╝╚", @"  /▒▒\", @"   $_$");
            thAndrea.Start(andreaData);
            thBaldo.Start(baldoData);
            thCarlo.Start(carloData);
            Stato();
            
            int cont = 0;
            do
            {
                Scrivi(0, 15, "    Scegliere l'operazione da eseguire:    ");
                Scrivi(30, 16, "   Join - premere J       ");
                Scrivi(30, 17, "   Abort - premere A      ");
                Scrivi(30, 18, "   Suspend - premere S    ");
                Scrivi(30, 19, "   Resume - premere R     ");
                Stato();
                if (KeyAvailable)
                {
                  SetCursorPosition(31, 20);
                  Menu();
                }
            } while (thAndrea.IsAlive || thBaldo.IsAlive || thCarlo.IsAlive);
            Stato();
            Thread.Sleep(300);
            int clean = 0;
            while (clean < WindowHeight)
            {
                Scrivi(0, 0, "                                                                                                                         ");
            }
            SetCursorPosition(WindowWidth / 2 - 26, WindowHeight / 2);
            Write("PREMERE QUALSIASI TASTO PER FINIRE LA GARA");
            ReadKey();
        }

    }
}
