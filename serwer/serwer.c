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
#include <json.h>
#define SERVER_PORT 5501
#define QUEUE_SIZE 10
#define MAX_CLIENTS 10


pthread_mutex_t lock;
//struktura zawierająca dane, które zostaną przekazane do wątku
struct thread_data_t {
 //TODO
 int deskr;
 int *tab_deskr;
};


//Wypisuje wartosci przypisane do klucza
void wypisywanieOgolne(json_object *jobj){
	json_object_object_foreach(jobj,key,val){
		printf("%s: %s\n", key ,
			json_object_get_string(val));;
	}
}


char * przydzielNumer(){
	json_object *jobjFromFile;
	jobjFromFile = json_object_new_object();
	jobjFromFile = json_object_from_file("registered.txt");	
	char * tmp;
	tmp = "0";
	char str_new[1024];
	json_object_object_foreach(jobjFromFile,key,val){
		val = val; //code just to erase warning
		tmp = key;
	}
	if(strcmp("0",tmp) == 0){
		json_object_put(jobjFromFile);
		return "1";
	}
	else{
		strcpy(str_new,tmp);
		char *end;
		//Zamiana string na int i inkrementacja
		int i = (int)strtol(str_new, &end, 10);
		i = i+1;
		//Zamiana int na string
		sprintf(str_new, "%d", i);
		tmp = str_new;
		json_object_put(jobjFromFile);
		return tmp;
	}
}

struct json_object * obsluzRejestracje(json_object * jobj){
	struct json_object * jobjReturn;
	struct json_object * jobjDoPliku;
	jobjReturn = json_object_new_object();
	char *end;
	char numer[1024];
	strcpy(numer,przydzielNumer());
	int i = (int)strtol(numer, &end, 10);
	if(i <= MAX_CLIENTS){
		json_object_object_add(jobjReturn, "recipient", json_object_new_string("null"));
		json_object_object_add(jobjReturn, "sender", json_object_new_string("0"));
		json_object_object_add(jobjReturn, "type", json_object_new_string("registration_success"));
		json_object_object_add(jobjReturn, "content", json_object_new_string(numer));
		jobjDoPliku = json_object_from_file("registered.txt");
		json_object_object_add(jobjDoPliku,numer,json_object_object_get(jobj,"content"));
		json_object_to_file("registered.txt",jobjDoPliku);
		json_object_put(jobjDoPliku);
		return(jobjReturn);
	}
	else{
		json_object_object_add(jobjReturn, "recipient", json_object_new_string("null"));
		json_object_object_add(jobjReturn, "sender", json_object_new_string("0"));
		json_object_object_add(jobjReturn, "type", json_object_new_string("registration_failure"));
		json_object_object_add(jobjReturn, "content", json_object_new_string("too_many_registred_clients"));
		return(jobjReturn);
	}
}


struct json_object * obsluzLogowanie(json_object * jobj){
	json_object * jobjReturn, *jobjFromFile, *jobjToFile;
	jobjReturn = json_object_new_object();
	jobjFromFile = json_object_new_object();
	jobjFromFile = json_object_from_file("registered.txt");
	char login[1024];
	char password[1024];
	strcpy(login,json_object_get_string(json_object_object_get(jobj,"sender")));
	strcpy(password,json_object_get_string(json_object_object_get(jobj,"content")));
	json_object_object_foreach(jobjFromFile,key,val){
		//sprawdzanie loginu
		if(strcmp(login,key) == 0 ){
			//sprawdzanie hasla
			if(strcmp(password,json_object_get_string(val)) == 0){
				jobjToFile = json_object_new_object();
		        jobjToFile = json_object_from_file("logged_in.txt");
				json_object_object_add(jobjToFile, login, json_object_new_string(password));
				json_object_to_file("logged_in.txt",jobjToFile);
				json_object_put(jobjToFile);
				//wypelnienie wiadomosci i jej zwrocenie
				json_object_object_add(jobjReturn, "recipient", json_object_new_string(login));
				json_object_object_add(jobjReturn, "sender", json_object_new_string("0"));
				json_object_object_add(jobjReturn, "type", json_object_new_string("login_success"));
				json_object_object_add(jobjReturn, "content", json_object_new_string("null"));
				json_object_put(jobjFromFile);
				json_object_put(jobjToFile);
				return jobjReturn;
			}
			break;
		}
	}
	//login lub haslo nie zgadzaja sie;
	json_object_object_add(jobjReturn, "recipient", json_object_new_string(login));
	json_object_object_add(jobjReturn, "sender", json_object_new_string("0"));
	json_object_object_add(jobjReturn, "type", json_object_new_string("login_failed"));
	json_object_object_add(jobjReturn, "content", json_object_new_string("wrong_login_or_password"));
	json_object_put(jobjFromFile);
	return jobjReturn;
}

struct json_object * obsluzWylogowanie(json_object * jobj){
	json_object * jobjReturn, *jobjFromFile, *jobjToFile;
	jobjReturn = json_object_new_object();
	jobjFromFile = json_object_new_object();
	jobjFromFile = json_object_from_file("logged_in.txt");
	char login[1024];
	char password[1024];
	strcpy(login,json_object_get_string(json_object_object_get(jobj,"sender")));
	strcpy(password,json_object_get_string(json_object_object_get(jobj,"content")));
	json_object_object_foreach(jobjFromFile,key,val){
		//sprawdzanie loginu
		if(strcmp(login,key) == 0 ){
			//sprawdzanie hasla
			if(strcmp(password,json_object_get_string(val)) == 0){
				jobjToFile = json_object_new_object();
				//wypelnienie wiadomosci i jej zwrocenie
				json_object_object_add(jobjReturn, "recipient", json_object_new_string(login));
				json_object_object_add(jobjReturn, "sender", json_object_new_string("0"));
				json_object_object_add(jobjReturn, "type", json_object_new_string("logout_success"));
				json_object_object_add(jobjReturn, "content", json_object_new_string("null"));
				//usuwanie uzytkownika z rejestru zalogowanych
		        jobjToFile = json_object_from_file("logged_in.txt");
				json_object_object_del(jobjToFile, login);
				json_object_to_file("logged_in.txt",jobjToFile);
				json_object_put(jobjToFile);
				json_object_put(jobjFromFile);
				json_object_put(jobjToFile);
				return jobjReturn;
			}
			break;
		}
	}
	//login lub haslo nie zgadzaja sie;
	json_object_object_add(jobjReturn, "recipient", json_object_new_string(login));
	json_object_object_add(jobjReturn, "sender", json_object_new_string("0"));
	json_object_object_add(jobjReturn, "type", json_object_new_string("logout_failed"));
	json_object_object_add(jobjReturn, "content", json_object_new_string("wrong_login_or_password"));
	json_object_put(jobjFromFile);
	return jobjReturn;
}

struct json_object * obsluzWiadomosc(json_object * jobj){
	json_object * jobjReturn, *jobjFromFile;
	jobjReturn = json_object_new_object();
	jobjFromFile = json_object_new_object();
	jobjFromFile = json_object_from_file("logged_in.txt");
	char login_nadawcy[1024];
	char login_odbiorcy[1024];
	strcpy(login_nadawcy,json_object_get_string(json_object_object_get(jobj,"sender")));
	strcpy(login_odbiorcy,json_object_get_string(json_object_object_get(jobj,"recipient")));
	json_object_object_foreach(jobjFromFile,key,val){
		val = val; //only to remove warning
		//sprawdzanie loginu nadawcy
		if(strcmp(login_nadawcy,key) == 0 ){
			//sprawdzanie loginu odbiorcy
			json_object_object_foreach(jobjFromFile,key2,val2)
			{
				if(strcmp(login_odbiorcy,key2) == 0){
					val2 = val2; //only to remove warning
				//wypelnienie wiadomosci i jej zwrocenie
				json_object_object_add(jobjReturn, "recipient", json_object_new_string(login_odbiorcy));
				json_object_object_add(jobjReturn, "sender", json_object_new_string(login_nadawcy));
				json_object_object_add(jobjReturn, "type", json_object_new_string("message"));
				json_object_object_add(jobjReturn, "content", json_object_object_get(jobj,"content"));
				json_object_put(jobjFromFile);
				return jobjReturn;
				}
			}
			//RECIPIENT FAILURE
			json_object_object_add(jobjReturn, "recipient", json_object_new_string(login_nadawcy));
			json_object_object_add(jobjReturn, "sender", json_object_new_string("0"));
			json_object_object_add(jobjReturn, "type", json_object_new_string("message"));
			json_object_object_add(jobjReturn, "content", json_object_new_string("recipient_not_logged_in"));
			json_object_put(jobjFromFile);
			return jobjReturn;

		}
	}
	//login nadawcy nie zgadza sie;
	json_object_object_add(jobjReturn, "recipient", json_object_new_string(login_nadawcy));
	json_object_object_add(jobjReturn, "sender", json_object_new_string("0"));
	json_object_object_add(jobjReturn, "type", json_object_new_string("error"));
	json_object_object_add(jobjReturn, "content", json_object_new_string("sender_not_logged_in"));
	json_object_put(jobjFromFile);
	return jobjReturn;
}

struct json_object * obslugaWiadomosci(json_object * jobj){
	//wypisz wiadomosc
	printf("\nReceieved:\n");
	wypisywanieOgolne(jobj);
	struct json_object * jobjContent;
	jobjContent = json_object_object_get(jobj,"type");
	char message_type[1024];
	strcpy(message_type,json_object_to_json_string(jobjContent));
	if(strcmp(message_type,"\"registration_request\"") == 0){
		jobj = obsluzRejestracje(jobj);
	} 
	else if(strcmp(message_type,"\"login_request\"") == 0){
		jobj = obsluzLogowanie(jobj);
	} 
	else if(strcmp(message_type,"\"logout_request\"") == 0){
		jobj = obsluzWylogowanie(jobj);
	} 
	else if(strcmp(message_type,"\"message\"") == 0){
		jobj = obsluzWiadomosc(jobj);
	} 
	else
	{
		printf("Message error: Type not qualified \n");
	}
	printf("\nSent:\n");
	wypisywanieOgolne(jobj);
	return jobj;
}


//funkcja opisującą zachowanie wątku
void *ThreadBehavior(void *t_data)
{
    pthread_detach(pthread_self());
    struct thread_data_t *th_data = (struct thread_data_t*)t_data;
    struct json_object *jobj , *outcoming_jobj;
    outcoming_jobj = json_object_new_object();

    char bufor[1024];
    int rc;
    int* tab_deskr = (*th_data).tab_deskr;
    printf("\nNew connection (descr.): %d\n", (*th_data).deskr);
    while(1) {
        rc = read ((*th_data).deskr, bufor, 1024); //odczytaj dane do bufora
		pthread_mutex_lock(&lock); //zamknij mutex
        if(rc > 0) {
          printf("\n(received from client %d): %s\n", (*th_data).deskr, bufor);
        
			//wiadomosc jest przeksztalcana do formatu json
			jobj = json_tokener_parse(bufor);
		
			//obsluga komunikatu
			outcoming_jobj = obslugaWiadomosci(jobj);
			//bufor jest czyszczony iwiadomosc ladowana jest do bufora
			memset(bufor,0,sizeof(bufor));
			strcpy(bufor,json_object_get_string(outcoming_jobj));

			//odeslanie wiadomosci do klientow
			//TODO nie do wszystkich, tylko do konkretnego
			if(*(tab_deskr) != 0) {
				write(*(tab_deskr), bufor, 1024);
				printf("\n(sent to client %d): %s\n", *(tab_deskr), bufor);
			}
		memset(bufor,0,sizeof(bufor));

		}
		pthread_mutex_unlock(&lock);	
		sleep(1);
    }

    pthread_exit(NULL);
}


//funkcja obsługująca połączenie z nowym klientem
void handleConnection(int connection_socket_descriptor, int* tab_deskr) {
    //wynik funkcji tworzącej wątek
    int create_result = 0;
    pthread_t thread1;

    //uchwyt na wątek
    struct thread_data_t *struktura1 = malloc(sizeof(struktura1));
    struktura1->deskr = connection_socket_descriptor;
    struktura1->tab_deskr = tab_deskr;

    create_result = pthread_create(&thread1, NULL, ThreadBehavior, (void *)struktura1);
    if (create_result){
    	printf("Błąd przy próbie utworzenia wątku, kod błędu: %d\n", create_result);
        exit(-1);

    }

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

   //inicjalizacja mutexa
     if (pthread_mutex_init(&lock, NULL) != 0)
    {
        printf("\n mutex init failed\n");
        return 1;
    }

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
		sleep(1);    
   }
   
   close(server_socket_descriptor);
   pthread_mutex_destroy(&lock);
   return(0);
}
