using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server 
             
{
    private static Socket listener;
    private static List<ClientHandler> clients = new List<ClientHandler>();     
    private static List<string> connectedClients = new List<string>();
   
    public static void Start()
    {
        // Creazione ed assegnazione di valori ad un server
        listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000));
        listener.Listen(10); // Specifica il numero massimo di persone che si possono collegare.
        Console.WriteLine("Server started.");

        while (true)
        {
            Socket clientSocket = listener.Accept(); // Ciclo che è sempre in azione(cioé server è sempre in ascolto) di acettare un nuovo client.
            ClientHandler clientHandler = new ClientHandler(clientSocket);  // Viene creato un clienthandler per ogni nuovo client.
            clients.Add(clientHandler); // Aggiunge un client nella lista.

            Thread clientThread = new Thread(clientHandler.HandleClient); // Thread che si occupa di comunicazione.
            clientThread.Start(); // Fa partire il thread.
        }
    }

    public static void BroadcastMessage(string message) // Messaggio che viene inviato a tutti client
    {
        foreach (var client in clients)
        {
            client.Send(message);
        }
    }

    public static void BroadcastConnectionStatus(string nickname, bool isConnected) // Messaggio che viene inviato a tutti, riguardo allo stato di conessione di un client.
    {
        string statusMessage = "";
        if (isConnected) // Verifica se c'è stata la connessione o la disconnessione, tramite un boolean.
        {
            connectedClients.Add(nickname); // Qualcuno si è connesso.
            statusMessage = $"{nickname} has joined";
            foreach (var client in clients)
            {
                client.Send(statusMessage);
            }
            if (connectedClients.Count == 1) // Verifica se il client è primo a collegarsi nella chat.
            {
                BroadcastMessage("You are the first one to join the chat.");
            }
            else // In caso se ci sono alte persone collegate, a tutti i client viene aggiornata la lista dei persone collegate.
            {
                string clientList = "People already connected to the chat:";
                foreach (var client in connectedClients)
                {
                    clientList += $"\n- {client}";
                }
                BroadcastMessage(clientList);
            }
        }
        else // Se if è false, quindi il client si è dissconesso.
        {
            statusMessage = $"{nickname} has left";
            connectedClients.Remove(nickname); // Rimuove il client dalla lista dei client connessi.
            BroadcastMessage(statusMessage); // Manda il messaggio a tutti che un client si è disconnesso.

            // Aggiorna la listBox sul server
            UpdateConnectedClientsListBox();
        }
    }

    private static void UpdateConnectedClientsListBox() // Si aggiorna la lista dei client connessi.
    {
        string clientList = "People currently connected to the chat:";
        foreach (var client in connectedClients)
        {
            clientList += $"\n- {client}";
        }

        BroadcastMessage(clientList);
    }

    public static void RemoveClient(ClientHandler client) // Metodo che rimuove un client dalla lista.
    {
        clients.Remove(client);
    }
}

class ClientHandler
{
    private Socket clientSocket;
    private string nickname;
    private string ip;
    private DateTime connectionTime;

    public ClientHandler(Socket clientSocket) // Costruttore della classe.
    {
        this.clientSocket = clientSocket;
        ip = ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString();
        connectionTime = DateTime.Now;
    }

    public void HandleClient()
    {
        byte[] buffer = new byte[1024];
        int bytesRead = clientSocket.Receive(buffer);
        nickname = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim(); // Trim cancella gli spazi vuoti all'inizio o alla fine del messaggio
                                                                          // e quindi il nickname del client si prende dalla listbox inviato al server.
        Console.WriteLine($"New client connected: {nickname}, {ip}, {connectionTime}"); 
        Server.BroadcastConnectionStatus(nickname, true); // Scrive a tutti che si è collegato un client.

        while (true) // Il ciclo infinito che riceve i dati da client.
        {
            try // Accetta il messaggio e lo decodifica secondo il protocollo <EOF>
            {
                bytesRead = clientSocket.Receive(buffer);
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                if (message.EndsWith("<EOF>"))
                {
                    message = message.Substring(0, message.Length - "<EOF>".Length);
                }

                Console.WriteLine($"[{nickname}]: {message}");

                Server.BroadcastMessage($"[{nickname}]: {message}");
            }
            catch // Eccezione in cui il client interrompe la comunicazione e il try entra nella catch dove esegue l'uscità dal ciclo.
            {
                Console.WriteLine($"Client disconnected: {nickname}, {ip}, {DateTime.Now}");
                Server.RemoveClient(this);
                Server.BroadcastConnectionStatus(nickname, false);
                break;
            }
        }
        // C'è stata la disconnessione e quindi il server sospende i due socket di comunicazione e gli chiude.
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close(); 
    }

    public void Send(string message) // Manda il messaggio che viene passato al metodo al client tramite socket
    {
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        clientSocket.Send(buffer);
    }
}



class Program
{
    static void Main(string[] args)
    {
        Server.Start(); // Invia il server.
    }
}
