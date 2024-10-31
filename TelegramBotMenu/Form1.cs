using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TelegramLibrary;
using System.Globalization;

namespace TelegramBotMenu
{
    public partial class Form1 : Form
    {
        // Enter Telegram bot API URL
        string Url = "";
        WebClient client;
        int LastUpdateId = 0;
        public Form1()
        {
            InitializeComponent();
            client = new WebClient();
            Task.Run(() => { UpdateAll(); });
        }
        private void UpdateAll()
        {
            while(true) 
            {
                string address = Url + "/getUpdates?offset=" + (LastUpdateId + 1);
                string str = client.DownloadString(address);
                TeleMessage msg = JsonSerializer.Deserialize<TeleMessage>(str);
                if (msg.ok || msg.result.Length > 0)
                    foreach (var item in msg.result)
                    {
                        LastUpdateId = item.update_id;
                        if (item.message != null)
                            AnswerIsMessage(item);
                        if (item.callback_query != null)
                            AnswerIsQuery(item);          
                    }
            }
        }
        private void AnswerIsQuery(Result item)
        {
            WriteLog(item.callback_query.data);
            AnswerToQuery(item);
        }
        private void AnswerIsMessage(Result item)
        {
            WriteLog(item.message.text);
            DateTime time = UnixSecondsToDateTime(item.message.date);
            textBox1.BeginInvoke(new Action(() => { WriteLog(time.AddHours(3).ToShortTimeString() + ": " + item.message.text); }));
            SendAnswer(item.message.chat.id, item.message.text);
        }
        private void WriteLog(string str) => textBox1.Text += str + Environment.NewLine;
        public DateTime UnixSecondsToDateTime(long timestamp, bool local = false)
        {
            var offset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            return local ? offset.LocalDateTime : offset.UtcDateTime;
        }
        private void SendAnswer(long chat_id, string message)
        {
            string answer = $"\"{message}\" является неизвестной коммандой!";
            switch (message.ToLower())
            {
                case "/start":
                    answer = "Я телеграм Бот \U0001F450. Есть вопросы /help";
                    break;
                case "/help":
                    answer = "/help - список комманд\n/start - начать работу с ботом\n/menu - показать меню\n/deletemenu - убрать меню\n/inlinemenu - меню в сообщении";
                    break;
                case "/menu":
                    MyMenu(chat_id);
                    return;
                case "/deletemenu":
                    {
                        NameValueCollection nvc = new NameValueCollection();
                        nvc.Add("chat_id", chat_id.ToString());
                        nvc.Add("text", "Меню удалено");
                        answer = JsonSerializer.Serialize(new RemoveButtons());
                        nvc.Add("reply_markup", answer);
                        client.UploadValues(Url + "/sendMessage", nvc);
                    }
                    return;
                case "/inlinemenu":
                    InlineMenu(chat_id);
                    return;
            }
            SendMessage(chat_id, answer);
        }
        private void InlineMenu(long chat_id)
        {
            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();
            
            for(int i = 1; i <= 12; i++)
            {
                List<InlineKeyboardButton> button = new List<InlineKeyboardButton>() 
                { 
                    new InlineKeyboardButton(new DateTime(2000, i, 1).ToString("MMMM", new System.Globalization.CultureInfo("ru-RU")), i.ToString()) 
                };
                buttons.Add(button);
            }

            InlineKeyboard keyboard = new InlineKeyboard(buttons);
            string replyMarkup = JsonSerializer.Serialize(keyboard);

            SendMessage(chat_id, "inline month menu", replyMarkup);
        }
        private void AnswerToQuery(Result item)
        {
            string address = Url + "/answerCallbackQuery";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("callback_query_id", item.callback_query.id);
            nvc.Add("text", "Ага");
            client.UploadValues(address, nvc);
        }
        private void MyMenu(long chat_id)
        {
            string address = Url + "/sendMessage";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("chat_id", chat_id.ToString());
            nvc.Add("text", "Проверка меню");
            List<string> keyboardLine1 = new List<string>() 
            {
                "/start",
                "/help"
            };
            List<string> keyboardLine2 = new List<string>() 
            {
                "/deletemenu",
                "/inlinemenu"
            };
            List<string> keyboardLine3 = new List<string>() 
            {
                "Заглушка 1",
                "🌅 Заглушка 2",
                "Заглушка 3"
            };
            List<List<string>> keyboard = new List<List<string>>
            {
                keyboardLine1, keyboardLine2, keyboardLine3
            };
            TeleButtons buttons = new TeleButtons(keyboard);
            string replyMarkup = JsonSerializer.Serialize(buttons);
            nvc.Add("reply_markup", replyMarkup);
            client.UploadValues(address, nvc);
        }
        private void SendMessage(long chat_id, string message, string replyMarkup = "")
        {
            string address = Url + "/sendMessage";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("chat_id", chat_id.ToString());
            nvc.Add("text", message);
            if (replyMarkup != null)
                nvc.Add("reply_markup", replyMarkup);
            client.UploadValues(address, nvc);
        }
    }
}
