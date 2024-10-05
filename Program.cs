using Telegram.Bot;
using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Translation.V2;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using OpenAI_API;
using System.Diagnostics.Eventing.Reader;
using OpenAI_API.Chat;
using TelegramBot;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using Telegram.Bots.Http;
using Telegram.Bots.Types;
using Update = Telegram.Bot.Types.Update;
using Telegram.Bots;
using System.Security.Cryptography.X509Certificates;

namespace TelegramBotExperiments
{
    delegate void Waiting();
    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("Your Key");
        static OpenAIAPI api = new OpenAIAPI("Your Key");
        static TranslationClient client = TranslationClient.CreateFromApiKey("Your Key");
        static ActionWithDatabases databaseAct = new ActionWithDatabases();




        static string? messageSend = "";
        static string? messageGet = "";
        static string? explainMessage = "explain like a 13 year old";
        static bool explainingEasly = false;
        static string? translatedTextQuestion = "";
        static string? translatedTextAnswer = "";
        static bool flag = false;
        static bool flagQuest = false;
        static bool flagAnswer = false;
        const string commandHelp = "/help";
        const string commandInfo = "/info";
        const string commandExplainEasly = "/explainEasly";
        const string commandStart = "/start";
        const string commandBuy = "/buy";
        const string commandTask = "/task";
        const string commandTokenCount = "/tokenCount";
        const string commandToken = "/token";
        const string commandGift = "/gift";
        const string commandAdvantage = "/advantage";

        const string tokenMessage = "Թոքենը դա  թվային միավոր է, որն օգտագործվում է որպես արժույթ 💵 բոտ-ծրագրում:\n" +
                                    "Այն հնարավորություն է տալիս օգտատերերին գնել ծառայություններ։ ⚡ \n" +
                                    "Դրանք հնարավոր է  ձեռք բերել գնումների միջոցով 💳 կամ վաստակել հարթակի ներսում հատուկ գործողությունների միջոցով📝:" +
                                    "\n1⃣ տոկենը = 1⃣ հարցմանը" +
                                    "\n/help- Ինչ եղանակներով կարելի է ստանալ տոկեններ 🆘";
        const string tokenCounntMessage = "Ինչպես ստանալ տոկեններ ❓ - /help \nՁեր տոկենների քանակն է`";
        const string infoMessage = "\nՀասանելի հրամնները ⤵" +
                                   "\n/explainEasly [հարցը...] - Տրված հարցը բացատրվում է մաքսիմալ հեշտ ձևով։✅" +
                                   "\n\n" +
                                   "/token - Ինչ է տոկենը❓\"" +
                                   "\n/advantage - HayqGPTBot-ի առավելությունը ChatGPT-ից 🚀" +
                                   "\n/help- Ինչ եղանակներով կարելի է ստանալ տոկեններ 🆘" +
                                   "\n/buy - Ինպես գնել տոկեններ 💳" +
                                   "\n/task  Ինչպես աշխատել տոկեններ🤷" +
                                   "\n\n/tokenCount Ստուգել տոկենների քանակը 💼" +
                                   "\n/gift - Ստանալ նվեր 3 տոկեն🎁";
        const string advantageMessage = "HayqGPT-ին հատուկ մշակված ալգորիթմի շնորհիվ հայատառ տեքստերը " +
                                        "կարողանում է 90 % -ով ավելի 📊արդյունավետ գեներացնել քան ChatGPT ";
        const string helpMessage = "Տոկեններ կարելի է ստանալ  նշված ձևերով⤵\n" +
                                   "1⃣ Գնել տոկեններ - մանրամասներին ծանոթանալու համար օգտագործեք /buy հրամանը💵 🔜 \n" +
                                   "2⃣ Սպասել վերալիցքավորմանը - ամեն օր օգտատերերի հաշիվը վերալիցքավորվում է 3 տոկենով /gift🎁 \n" +
                                   "3⃣ Կատարել հանձնարարություններ - որի դիմաց կպարգևատրվեք տոկեններով \n" +
                                   "մանրամասներին ծանոթանալու համար օգտագործեք /task հրամանը🔜";
       
        const string firstMessage = "Ողջույն ես  HayqGPT-ն եմ 👋 🇦🇲" +
                                    "\nՊատրաստ եմ պատասխանել քո 🇦🇲 հայատառ հարցերին ☺️  " +
                                    "\nՍեղմիր ❓/info հրամաններին ծանոթանալու համար";
        const string buyMessage = "Գտնվում է մշակման փուլում 🔜";
        const string taskMessage = "Գտնվում է մշակման փուլում 🔜";
        const string giftMessage = "Շնորհավորում եմ 🎉 \nԴուք ստացել եք  նվեր 3 տոկեն🎁 \n /token - Ինչ է տոկենը և ինչպես օգտագործել ❓ ";
        static string userTelegramId= string.Empty;
        static int giftCoin = 3;
        
                                    
        const string waitingURL = "https://cutewallpaper.org/24/hourglass-animated-gif/animated-svg-hourglass-preloader-by-tony-thomas-for-medialoot-on-dribbble.gif";
      
    

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {   
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            var message = update.Message;
        
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>((Newtonsoft.Json.JsonConvert.SerializeObject(update)));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message  && OnlyArmenian(update.Message.Text))
            {
                userTelegramId = myDeserializedClass.message.from.id.ToString();
                messageGet = message.Text;
                ActionWithDatabases.InsertJsonValuesOnInformationTable(myDeserializedClass);
                bool userExist = ActionWithDatabases.CheckingUsername(myDeserializedClass.message.from.id.ToString());
                if (userExist)
                {                                                    
                    int tokenCoin = ActionWithDatabases.ReturningTokenCount(myDeserializedClass.message.from.id.ToString());

                    MessageTextRecognaze(botClient, update, cancellationToken, myDeserializedClass, message);
                }
                else
                {
                    ActionWithDatabases.InsertingTokenCountAndUser(myDeserializedClass.message.from.id.ToString(), giftCoin);
                    await botClient.SendTextMessageAsync(message.Chat, giftMessage);
                    await botClient.SendTextMessageAsync(message.Chat, firstMessage);
                }             
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat, "Կխնդրեմ հարցերը գրել ՀԱՅԱՏԱՌ!!🇦🇲 և  հստակ😎");
            }
        }
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
        public static async void MessageTextRecognaze(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken,Root myDeserializedClass, Telegram.Bot.Types.Message message)
        {
            int tokenCoin = ActionWithDatabases.ReturningTokenCount(myDeserializedClass.message.from.id.ToString());
            if (message.Text.ToLower() == commandStart.ToLower())
            {
                await botClient.SendTextMessageAsync(message.Chat, firstMessage);
            }
            else if (message.Text.ToLower() == commandInfo.ToLower())
            {
                await botClient.SendTextMessageAsync(message.Chat, infoMessage);
            }
            else if (message.Text.ToLower() == commandHelp.ToLower())
            {
                await botClient.SendTextMessageAsync(message.Chat, helpMessage);
            }
            else if (message.Text.ToLower() == commandBuy.ToLower())
            {
                await botClient.SendTextMessageAsync(message.Chat, buyMessage);
            }
            else if (message.Text.ToLower() == commandTask.ToLower())
            {
                await botClient.SendTextMessageAsync(message.Chat, taskMessage);
            }
            else if (message.Text.ToLower() == commandTokenCount.ToLower())
            {
                int tokenCount = ActionWithDatabases.ReturningTokenCount(userTelegramId);
                await botClient.SendTextMessageAsync(message.Chat, $"{tokenCounntMessage}  {tokenCoin}");
            }
            else if (message.Text.ToLower() == commandToken.ToLower())
            {
                await botClient.SendTextMessageAsync(message.Chat, tokenMessage);
            }
            else if (message.Text.ToLower() == commandGift.ToLower())
            {
                DateTime time = ActionWithDatabases.SelectingTokenUpdateingTime(myDeserializedClass.message.from.id.ToString());
                DateTime updateTime = DateTime.Today;
                updateTime = Convert.ToDateTime(($"{updateTime.Day}.{updateTime.Month}.{updateTime.Year} 23:59:59"));
                Console.WriteLine(time.ToString());
                if (time.Day < DateTime.Now.Day)
                {
                    if (tokenCoin >= giftCoin)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, $"Տոկենների քանակը {giftCoin} և շատ լինելու դեպքում անվճար տոկենները չեն վերալիցքավորվում 😢");
                    }
                    else
                    {
                        ActionWithDatabases.GiftEveryDayToken(myDeserializedClass.message.from.id.ToString(), giftCoin, giftCoin);
                        await botClient.SendTextMessageAsync(message.Chat, giftMessage);
                    }

                }
                else
                {
                    TimeSpan timeForGift = updateTime.Subtract(DateTime.Now);
                    await botClient.SendTextMessageAsync(message.Chat, $"🎁Անվճար տոկեններ ստանալուն մնացել է՝ {timeForGift.Hours} ժ. {timeForGift.Minutes} րոպե⏳");
                }
            }
            else if (message.Text.ToLower() == commandAdvantage.ToLower())
            {
                await botClient.SendTextMessageAsync(message.Chat, advantageMessage);
                await botClient.SendTextMessageAsync(message.Chat, "✅HayqGPT-ի գեներացման արդյունքը");

                await botClient.SendPhotoAsync(message.Chat, Telegram.Bot.Types.InputFile.FromUri("https://ibb.co/Q9nsz6k"));
                await botClient.SendTextMessageAsync(message.Chat, "❌ChatGPT-ի գեներացման արդյունքը");

                await botClient.SendPhotoAsync(message.Chat, Telegram.Bot.Types.InputFile.FromUri("https://ibb.co/fD06mkp"));

            }
            else
            if (tokenCoin > 0)
            {
                if (message.Text.Contains(commandExplainEasly))
                {
                    CommandExplainEasly(botClient, message);
                }

                else
                {
                    var messageId = await bot.SendAnimationAsync(myDeserializedClass.message.chat.id, Telegram.Bot.Types.InputFile.FromUri($"{waitingURL}"));
                    if (explainingEasly == true)
                    {
                        messageGet += explainMessage;
                        explainingEasly = false;
                    }
                    OpenAiMethod();
                    WaitingAiAnswerTranslation(myDeserializedClass, botClient, message, messageId);
                }
            }
            else
            {
                //Xndrumenq licqavorel dzer hashivy
                await botClient.SendTextMessageAsync(message.Chat, "Ձեր տոկենները սպառվել են 😥 \nՍեղմեք /help հավելյալ ինֆորմացիա ստալու համար");
            }
        }
    
        public static  void CommandExplainEasly(ITelegramBotClient botClient ,Telegram.Bot.Types.Message message)
        {

            if (message.Text == commandExplainEasly)
            {
                botClient.SendTextMessageAsync(message.Chat, " Որն է ձեր հարցը՞");

                explainingEasly = true;              
            }
            else
            {
                messageGet += explainMessage;

                OpenAiMethod();
                for (int i = 0; i < 60; i++)
                {
                    Waitig(1000);
                    if (flag == true)
                    {
                         botClient.SendTextMessageAsync(message.Chat, translatedTextAnswer);
                        messageGet = "";
                        messageSend = "";
                        flag = false;                                      
                        break;
                    }
                }
            }
        }
        public static async Task GoogleTranslateQuestion( string fromLanguage,string toLanguage, string text ) 
        {// translated question   before sending to OpenAI
            var response = await client.TranslateTextAsync(text, toLanguage, fromLanguage);
            translatedTextQuestion = response.TranslatedText;
            flagQuest = true;                                   
        }

        public static async  Task GoogleTranslateAnswer( string fromLanguage, string toLanguage, string text )
        {//translated answer after sending OpenAI             
            for (int i = 0; i < 60; i++)
            {
                if (flag == true)
                {
                    var response = await client.TranslateTextAsync(text, fromLanguage, toLanguage);
                    translatedTextAnswer = response.TranslatedText;
                    break;
                }
                else
                {
                    Console.WriteLine("null");
                }
            }
        }
        public static async void WaitingAiAnswerTranslation(Root myDeserializedClass, ITelegramBotClient botClient,Telegram.Bot.Types.Message message,Telegram.Bot.Types.Message messageId) 
        {
            for (int i = 0; i < 60; i++)
            {
                Waitig(1000);
                if (flag == true && flagAnswer == true)
                {
                    DeletingWaitingAnimation(myDeserializedClass,botClient,messageId);
                    await botClient.SendTextMessageAsync(message.Chat, translatedTextAnswer);

                    PuttingDefaultValues();
                    break;
                }
            }            
        }

        public static void DeletingWaitingAnimation(Root myDeserializedClass, ITelegramBotClient botClient, Telegram.Bot.Types.Message messageId)
        {
            int id = messageId.MessageId;
            botClient.DeleteMessageAsync(myDeserializedClass.message.chat.id, id);  

        }

        public static void PuttingDefaultValues() 
        {
            messageGet = "";
            messageSend = "";
            flag = false;
            flagAnswer = false;
        }
        public static void Waitig(int time)
        {
            Thread.Sleep(time);
        }

        
        public static bool BotCommands(string str) 
        {
            return str.Contains(commandExplainEasly) ||
                   str.Contains(commandStart) ||
                   str.Contains(commandHelp)  ||
                   str.Contains(commandBuy)   ||
                   str.Contains(commandTask)  ||
                   str.Contains(commandTokenCount) ||
                   str.Contains(commandToken) ||
                   str.Contains(commandGift) ||
                   str.Contains(commandAdvantage) ||
                   str.Contains(commandInfo);
        }
        public  static async void OpenAiMethod()
        {
            Console.WriteLine("START");
            
            await GoogleTranslateQuestion(LanguageCodes.Armenian,LanguageCodes.English, messageGet );
            Conversation conversation;
            string? response;
            conversation = api.Chat.CreateConversation();
            conversation.AppendUserInput(translatedTextQuestion);
            response = await conversation.GetResponseFromChatbotAsync();
            flag = true;
            ActionWithDatabases.SubtractionOperationFromTokenCount(userTelegramId);

            messageSend = response.ToString();
           
            await GoogleTranslateAnswer(LanguageCodes.Armenian, LanguageCodes.English,messageSend);
            
            flagAnswer = true;

            
            Console.WriteLine("END");

        }
        public static bool OnlyArmenian(string str) 
        { // Only Armenian Characters
            bool hasArmenianCharacters = str.Any(c => c >=0x0530 && c<= 0x058A );
            return  BotCommands(str)|| hasArmenianCharacters ;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, 
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}
