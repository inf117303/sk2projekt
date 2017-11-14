#include <pthread.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <netdb.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <stdio.h>

#define BUF_SIZE 1024
#define NUM_THREADS     5

//struktura zawierająca dane, które zostaną przekazane do wątku
struct thread_data_t
{
int deskr;
};

//wskaźnik na funkcję opisującą zachowanie wątku
void *ThreadBehavior(void *t_data)
{
    struct thread_data_t *th_data = (struct thread_data_t*)t_data;
    //dostęp do pól struktury: (*th_data).pole
    //TODO (przy zadaniu 1) klawiatura -> wysyłanie albo odbieranie -> wyświetlanie
    char bufor[100];
    printf("New connection (descr.): %d\n", (*th_data).deskr);
    
    while(1) {
        fgets (bufor, 100, stdin);        
        write((*th_data).deskr, bufor, 100);
        printf("(sent to server): %s", bufor);
    }

    pthread_exit(NULL);
}

void *ThreadBehavior2(void *t_data)
{
    struct thread_data_t *th_data = (struct thread_data_t*)t_data;
    int rc;
    //dostęp do pól struktury: (*th_data).pole
    //TODO (przy zadaniu 1) klawiatura -> wysyłanie albo odbieranie -> wyświetlanie
    char bufor[100];
    printf("deskr: %d\n", (*th_data).deskr);
    
    while(1) {  
        rc = read ((*th_data).deskr, bufor, 100);
        if(rc > 0) {
         printf("(received from server): %s", bufor);
        }
    }

    pthread_exit(NULL);
}


//funkcja obsługująca połączenie z serwerem
void handleConnection(int connection_socket_descriptor) {
    //wynik funkcji tworzącej wątek
    int create_result = 0;

    //uchwyt na wątek
    pthread_t thread1;

    //dane, które zostaną przekazane do wątku
    //struct thread_data_t t_data;
    
    struct thread_data_t *struktura1 = malloc(sizeof(struktura1));
    struktura1->deskr = connection_socket_descriptor;
    
    create_result = pthread_create(&thread1, NULL, ThreadBehavior, (void *)struktura1);
    if (create_result){
       printf("Błąd przy próbie utworzenia wątku, kod błędu: %d\n", create_result);
       exit(-1);
    }
    
    create_result = pthread_create(&thread1, NULL, ThreadBehavior2, (void *)struktura1);
    if (create_result){
       printf("Błąd przy próbie utworzenia wątku, kod błędu: %d\n", create_result);
       exit(-1);
    }

    //TODO (przy zadaniu 1) odbieranie -> wyświetlanie albo klawiatura -> wysyłanie
    while(1) { }
}


int main (int argc, char *argv[])
{
   int connection_socket_descriptor, connection_socket_descriptor2;
   int connect_result;
   struct sockaddr_in server_address;
   struct hostent* server_host_entity;

   if (argc != 3)
   {
     fprintf(stderr, "Sposób użycia: %s server_name port_number\n", argv[0]);
     exit(1);
   }

   server_host_entity = gethostbyname(argv[1]);
   if (! server_host_entity)
   {
      fprintf(stderr, "%s: Nie można uzyskać adresu IP serwera.\n", argv[0]);
      exit(1);
   }
   
   // wątek 1

   connection_socket_descriptor = socket(PF_INET, SOCK_STREAM, 0);
   if (connection_socket_descriptor < 0)
   {
      fprintf(stderr, "%s: Błąd przy probie utworzenia gniazda.\n", argv[0]);
      exit(1);
   }

   memset(&server_address, 0, sizeof(struct sockaddr));
   server_address.sin_family = AF_INET;
   memcpy(&server_address.sin_addr.s_addr, server_host_entity->h_addr, server_host_entity->h_length);
   server_address.sin_port = htons(atoi(argv[2]));

   connect_result = connect(connection_socket_descriptor, (struct sockaddr*)&server_address, sizeof(struct sockaddr));
   if (connect_result < 0)
   {
      fprintf(stderr, "%s: Błąd przy próbie połączenia z serwerem (%s:%i).\n", argv[0], argv[1], atoi(argv[2]));
      exit(1);
   }

   handleConnection(connection_socket_descriptor);

   close(connection_socket_descriptor);
   
   // wątek 2
   
   connection_socket_descriptor = socket(PF_INET, SOCK_STREAM, 0);
   if (connection_socket_descriptor < 0)
   {
      fprintf(stderr, "%s: Błąd przy probie utworzenia gniazda.\n", argv[0]);
      exit(1);
   }
   
   memset(&server_address, 0, sizeof(struct sockaddr));
   server_address.sin_family = AF_INET;
   memcpy(&server_address.sin_addr.s_addr, server_host_entity->h_addr, server_host_entity->h_length);
   server_address.sin_port = htons(atoi(argv[2]));

   connect_result = connect(connection_socket_descriptor2, (struct sockaddr*)&server_address, sizeof(struct sockaddr));
   if (connect_result < 0)
   {
      fprintf(stderr, "%s: Błąd przy próbie połączenia z serwerem (%s:%i).\n", argv[0], argv[1], atoi(argv[2]));
      exit(1);
   }

   handleConnection(connection_socket_descriptor2);

   close(connection_socket_descriptor2);
   
   
   return 0;

}
