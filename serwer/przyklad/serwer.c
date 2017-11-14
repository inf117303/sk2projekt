#include <sys/types.h>
#include <sys/wait.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <netdb.h>
#include <signal.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <pthread.h>

#define SERVER_PORT 2111
#define QUEUE_SIZE 5
#define MAX_CLIENTS 3

//struktura zawierająca dane, które zostaną przekazane do wątku
struct thread_data_t {
 //TODO 
 int deskr;
 int *tab_deskr;
};

//funkcja opisującą zachowanie wątku - musi przyjmować argument typu (void *) i zwracać (void *)
void *ThreadBehavior(void *t_data)
{
    pthread_detach(pthread_self());
    struct thread_data_t *th_data = (struct thread_data_t*)t_data;
    //dostęp do pól struktury: (*th_data).pole
    //TODO (przy zadaniu 1) klawiatura -> wysyłanie albo odbieranie -> wyświetlanie
    char bufor[100];
    int rc, i;
    int* tab_deskr = (*th_data).tab_deskr;
    printf("New connection (descr.): %d\n", (*th_data).deskr);
    while(1) {
        // czytaj wiadomość od klienta
        rc = read ((*th_data).deskr, bufor, 100);
        if(rc > 0) {
         printf("(received from client %d): %s", (*th_data).deskr, bufor);
        }
        // odeślij wiadomość do klientow
        
        
        for(i=0; i<MAX_CLIENTS; i++) {
            if(*(tab_deskr + i) != 0) {
                write(*(tab_deskr + i), bufor, 100);
                printf("(sent to client %d): %s", *(tab_deskr + i), bufor);
            }
            
        }
    }
    
    
    //

    pthread_exit(NULL);
}

//funkcja obsługująca połączenie z nowym klientem
void handleConnection(int connection_socket_descriptor, int* tab_deskr) {
    //wynik funkcji tworzącej wątek
    int create_result = 0;
    

    //uchwyt na wątek
    pthread_t thread1;

    //dane, które zostaną przekazane do wątku
    //TODO dynamiczne utworzenie instancji struktury thread_data_t o nazwie t_data (+ w odpowiednim miejscu zwolnienie pamięci)
    //TODO wypełnienie pól struktury
    struct thread_data_t *struktura1 = malloc(sizeof(struktura1));
    struktura1->deskr = connection_socket_descriptor;
    struktura1->tab_deskr = tab_deskr;

    
        create_result = pthread_create(&thread1, NULL, ThreadBehavior, (void *)struktura1);
        if (create_result){
            printf("Błąd przy próbie utworzenia wątku, kod błędu: %d\n", create_result);
            exit(-1);

    } 
    

    //TODO (przy zadaniu 1) odbieranie -> wyświetlanie albo klawiatura -> wysyłanie
    
}

int main(int argc, char* argv[])
{
   int server_socket_descriptor;
   int connection_socket_descriptor;
   int bind_result;
   int listen_result;
   char reuse_addr_val = 1;
   struct sockaddr_in server_address;
   
   int client_count = 0;
   int descriptors[MAX_CLIENTS] = {0};

   //inicjalizacja gniazda serwera
   
   memset(&server_address, 0, sizeof(struct sockaddr));
   server_address.sin_family = AF_INET;
   server_address.sin_addr.s_addr = htonl(INADDR_ANY);
   server_address.sin_port = htons(SERVER_PORT);

   server_socket_descriptor = socket(AF_INET, SOCK_STREAM, 0);
   if (server_socket_descriptor < 0)
   {
       fprintf(stderr, "%s: Błąd przy próbie utworzenia gniazda..\n", argv[0]);
       exit(1);
   }
   setsockopt(server_socket_descriptor, SOL_SOCKET, SO_REUSEADDR, (char*)&reuse_addr_val, sizeof(reuse_addr_val));

   bind_result = bind(server_socket_descriptor, (struct sockaddr*)&server_address, sizeof(struct sockaddr));
   if (bind_result < 0)
   {
       fprintf(stderr, "%s: Błąd przy próbie dowiązania adresu IP i numeru portu do gniazda.\n", argv[0]);
       exit(1);
   }

   listen_result = listen(server_socket_descriptor, QUEUE_SIZE);
   if (listen_result < 0) {
       fprintf(stderr, "%s: Błąd przy próbie ustawienia wielkości kolejki.\n", argv[0]);
       exit(1);
   }

   while(1)
   {
       if(client_count < MAX_CLIENTS) {
           connection_socket_descriptor = accept(server_socket_descriptor, NULL, NULL);
            if (connection_socket_descriptor < 0)
            {
                fprintf(stderr, "%s: Błąd przy próbie utworzenia gniazda dla połączenia.\n", argv[0]);
                exit(1);
            }

            handleConnection(connection_socket_descriptor, descriptors);
            descriptors[client_count] = connection_socket_descriptor;
            client_count++;
       }
       
       
       
   }

   close(server_socket_descriptor);
   return(0);
}
