using System;
using System.Net;
using System.IO;

namespace an631{
    class Program{
		static bool err=false;
		static string token,linkSend,linkAsk,linkUpd,key,id,path="c:/c#/an631/data/";//путь к директории с ботом
		
		static string find(string json,string key,bool isint=false){//поиск значения по ключу в json записанным строкой
			int a,b;
			if(isint){
				a=json.IndexOf("\""+key+"\":")+key.Length+3;
				string d=json.Substring(a,json.Length-a);
				b=d.IndexOf(",\"");
			}else{
				a=json.IndexOf("\""+key+"\":\"")+key.Length+4;
				string d=json.Substring(a,json.Length-a);
				b=d.IndexOf("\",\"");
			}
		return json.Substring(a,b);
		}

		static string request(string link){//сетевой запрос с получением json ответа строкой
			WebRequest request=WebRequest.Create(link);
			request.Credentials=CredentialCache.DefaultCredentials;
			HttpWebResponse response=(HttpWebResponse)request.GetResponse();
			Stream data=response.GetResponseStream();
			StreamReader reader=new StreamReader(data);
			string r=reader.ReadToEnd();
		reader.Close();
		data.Close();
		response.Close();
		return r;
		}

		static bool sendMsg(string peer,string text){
			Console.Write("sm");
			string answ=request(linkSend+peer+"&message="+text);
		return true;
		}

		static void getKey(){
			key=find(request(linkAsk),"key");
			linkUpd="https://lp.vk.com/wh"+id+"?act=a_check&key="+key+"&wait=1&mode=2&ts=";
		}

		static bool check(int ts){
			string answer=request(linkUpd+ts);
			if(answer.Length==(24+ts.ToString().Length)){
				return false;//пропуск цикла в main, чтобы не увеличивать TS, когда событий нет
			}
			if(answer.Contains("failed")){//допили потом 
				Console.WriteLine("ERROR");
				Console.Write(answer);
				if(answer[10]=='2'){//непроверенный элемент
					getKey();
					Console.WriteLine("попытка получить новый KEY");
				}else{
					err=true;
				}
				return false;
			}
			string msg=find(answer,"text");
			if(msg.Contains("кис")){
				sendMsg(find(answer,"peer_id",true),"мяу");
			}
		return true;
		}
		
        static void Main(string[] args){
			Console.WriteLine("startin");
			StreamReader ft=new StreamReader(path+"login.txt");//данные о боте
			id=ft.ReadLine();//в первой строке group_id
			token=ft.ReadLine();//во второй токен с достаточными уровнями доступа
		ft.Close();
			linkSend="https://api.vk.com/method/messages.send?access_token="+token+"&v=5.131&random_id=0&peer_id=";
			linkAsk="https://api.vk.com/method/groups.getLongPollServer?access_token="+token+"&group_id="+id+"&v=5.131";
			getKey();
			StreamReader fr=new StreamReader(path+"lasTS.txt");
			int TS=int.Parse(fr.ReadLine());
		fr.Close();
			while(true){
				if(check(TS)){
					TS++;
					StreamWriter fw=new StreamWriter(path+"lasTS.txt");
					fw.WriteLine(TS);
				fw.Close();
				}
				Console.Write(".");
				if(err){
					Console.WriteLine("работа прекращена с ошибкой");
				break;
				}
			}
        }
    }
}