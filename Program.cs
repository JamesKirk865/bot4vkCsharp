using System;
using System.Net;
using System.Threading;
using System.IO;
using System.Collections.Generic;

namespace an631{
    class Program{
		static bool err=false;
		static string token,linkSend,linkAsk,linkUpd,key,id,path="c:/c#/an631/data/";//путь к директории с ботом
		static StreamReader fr;
		static StreamWriter fw;
		
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
			Console.Write("'");
			string answ=request(linkSend+peer+"&message="+text);
		return true;
		}

		static string toLowercase(string src){
			string capt="QWERTYUIOPLKJHGFDSAZXCVBNM";
			string low="qwertyuioplkjhgfdsazxcvbnm";
			char[]r=src.ToCharArray();
			for(int i=0;i<src.Length;++i)
				if(capt.Contains(src[i]))
					r[i]=low[capt.IndexOf(src[i])];
			capt="ЁЙЦУКЕНГШЩЗХЪЭЖДЛОРПАВЫФЯЧСМИТЬБЮ";
			low="ёйцукенгшщзхъэждлорпавыфячсмитьбю";
			for(int i=0;i<src.Length;++i)
				if(capt.Contains(src[i]))
					r[i]=low[capt.IndexOf(src[i])];
		return new String(r);
		}

		static void getKey(){
			Console.Write("\nGETTING KEY:");
			key=find(request(linkAsk),"key");
			linkUpd="https://lp.vk.com/wh"+id+"?act=a_check&key="+key+"&wait=15&mode=2&ts=";
			Console.WriteLine("COMPLETE");
		}

		static string read(string file){
			fr=new StreamReader(file);
			string data=fr.ReadLine();
			fr.Close();
		return data;
		}
		
		static string read2end(string file){
			fr=new StreamReader(file);
			string data=fr.ReadToEnd();
			fr.Close();
		return data;
		}

		static void write(string file,string data){
			fw=new StreamWriter(file);
			fw.WriteLine(data);
			fw.Close();
		}

		static bool check(int ts){
			string answer=request(linkUpd+ts);
			if(answer.Length==(24+ts.ToString().Length)){
				return false;//пропуск цикла в main, чтобы не увеличивать TS, когда событий нет
			}
			if(answer.Contains("failed")){//допили потом 
				Console.WriteLine("ERROR");
				Console.Write(answer);
				if(answer[10]=='2')//непроверенный элемент
					getKey();
				else
					err=true;
				return false;
			}
			string msg=find(answer,"text"),user=find(answer,"from_id",true),peer=find(answer,"peer_id",true);
			msg=toLowercase(msg);
			if(msg.Contains("пинг"))
				sendMsg(peer,"понг");
		return true;
		}
		
		static void cycle(){
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
					Console.WriteLine("\nUNKNOWN ERROR");
				break;
				}
			}
		}
		
        static void Main(string[] args){
			Console.Write(DateTimeOffset.Now+"\nstartin");
			StreamReader ft=new StreamReader(path+"login.txt");//данные о боте
			id=ft.ReadLine();//в первой строке group_id
			token=ft.ReadLine();//во второй токен с достаточными уровнями доступа
		ft.Close();
			linkSend="https://api.vk.com/method/messages.send?access_token="+token+"&v=5.131&random_id=0&peer_id=";
			linkAsk="https://api.vk.com/method/groups.getLongPollServer?access_token="+token+"&group_id="+id+"&v=5.131";
			getKey();
			Thread core=new Thread(cycle);
			core.Start();//поток с ботом
			while(true){//основной поток проверяет ввод с клавиатуры. если нажата клавиша - завершение работы бота
				ConsoleKeyInfo cki=Console.ReadKey();
				if(cki.Key==ConsoleKey.Escape){
					core.Interrupt();
				break;
				}
			}
			Console.WriteLine(DateTimeOffset.Now+"\nSHUTTING DOWN");
        }
    }
}