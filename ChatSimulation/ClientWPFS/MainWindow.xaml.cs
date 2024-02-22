using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System; // using neccessari

namespace ClientWPFS
{
    public partial class MainWindow : Window // CHABANIUK ANDRII, 4H, Simulazione Client, 2024-02-15
    {                                        // Simulazione di un client che comunica con il server.
        private Socket clientSocket;
        private string nickname;
        private Thread receiveMessageThread;  // Thread per fare in modo che il programma non si ferma e continua ad ascoltare i messagi.

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e) // Bottone che si occupa di connettersi al server, dopo che è stato inserito il nome.
        {
            nicknameTextBox.Visibility = Visibility.Hidden;
            disconnectButton.Visibility = Visibility.Visible;
            messageTextBox.Visibility = Visibility.Visible;
            sendMessageButton.Visibility = Visibility.Visible;
            connectButton.Visibility = Visibility.Hidden; // Gestione dei bottoni/textbox dopo che un client si connette.
            try // Prova a collegarsi al server, se non riesce andrà in catch e scrive il messaggio nella messagebox di un errore.
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Crea un socket di comunicazione.
                clientSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000)); // Si conette al server

                nickname = nicknameTextBox.Text;
                byte[] nicknameData = Encoding.ASCII.GetBytes(nickname);
                clientSocket.Send(nicknameData); // Converte il nome in byte e lo manda al server.

                receiveMessageThread = new Thread(ReceiveMessages);
                receiveMessageThread.Start(); // Thread che gestisce i messaggi recevuti dal server, in modo che non si blocca l'esecuzione del programma.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during connection: {ex.Message}");
            }
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e) // Bottone che gestisce l'invio del messaggio al server.
        {
            try // Prova a collegarsi al server, se non riesce andrà in catch e scrive il messaggio nella messagebox di un errore.
            {
                string message = messageTextBox.Text;
                byte[] messageData = Encoding.ASCII.GetBytes(message + "<EOF>");
                clientSocket.Send(messageData); // trasforma il messaggio in bytes, aggiunge il protocollo e lo manda.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e) // Bottone che si occupa di disconessione dal server.
        {
            try // Prova a collegarsi al server, se non riesce andrà in catch e scrive il messaggio nella messagebox di un errore.
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close(); // Sospende i socket di comunicazione e gli chiude.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during disconnection: {ex.Message}");
            }
            finally // Chiude tutto il programma dopo la disconessione
            { 
                Application.Current.Shutdown();
            }
        }

        private void ReceiveMessages() // Il metodo che si occupa di ricevere i messaggi
        {
            byte[] buffer = new byte[1024]; // Array con i dati
            while (true)
            {
                try
                {
                    if (clientSocket == null || !clientSocket.Connected) // Verifica se il client è disconesso, se è disconesso esce dal ciclo e termina.
                    {
                        break;
                    }

                    int bytesRead = clientSocket.Receive(buffer);
                    string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();  // Legge i byte e gli trasforma in stringa, il metodo .Trim() --> cancella i spazzi vuoti all'inizio della stringa e alla fine

                    if (receivedData.StartsWith("People already connected to the chat:") || receivedData.StartsWith("People currently connected to the chat:")) // Se arrivano i messaggi che iniziano con quelle parole, 
                    {                                                                                                                                           // quindi è da aggiornare listbox.
                        Dispatcher.Invoke(() => // la possibilità che offre WPF .NET, di eseguire una parte del codice con le sue risorse. ((https://learn.microsoft.com/it-it/dotnet/api/system.windows.threading.dispatcher.invoke?view=windowsdesktop-8.0))
                        {
                            connectedClientsListBox.Items.Clear();
                            connectedClientsListBox.Items.Add(receivedData); // Pulisce e mette i client conessi nella listbox dedicata.
                        });
                    }
                    else
                    {
                        string message = receivedData.TrimEnd(); // Prende il messagio e toglie lo spazio vuoto alla fine se c'è.
                        if (message.EndsWith("<EOF>"))  // Controlla il messaggio, toglie <EOF> e lo aggiunge nella listbox. EOF sta per end of stream
                        {
                            message = message.Substring(0, message.Length - "<EOF>".Length);
                            Dispatcher.Invoke(() =>
                            {
                                messageListBox.Items.Add(message);
                            });
                        }
                        if (message.EndsWith("<CHC>")) // Controlla il messaggio, toglie <CHC> e lo aggiunge nella listbox. CHC sta per client has connected
                        {
                            message = message.Substring(0, message.Length - "<CHC>".Length);
                            Dispatcher.Invoke(() =>
                            {
                                messageListBox.Items.Add(message);
                            });
                        }
                        else if (message.EndsWith("<CHD>")) // Controlla il messaggio, toglie <CHD> e lo aggiunge nella listbox. CHD sta per client has disconnected
                        {
                            message = message.Substring(0, message.Length - "<CHD>".Length);
                            Dispatcher.Invoke(() =>
                            {
                                messageListBox.Items.Add(message);
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                messageListBox.Items.Add(message);  // Aggiunge il messaggio nella listbox.
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error receiving messages: {ex.Message}");
                    break;
                }
            }
        }
    }
}