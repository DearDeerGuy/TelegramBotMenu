using CoreTelegramLibrary;
using CoreTelegramLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using TelegramLibrary.Models;

namespace CoreStorage
{
    enum BotState
    {
        wait,
        send
    }
    public class TeleClass
    {
        string Url = "https://api.telegram.org/bot6087371994:AAGX1NxyEvRBbRB29tDN5uDhNE5GAQLecMk";
        WebClient client;
        int LastUpdateId = 0;
        TextBox textBox;
        DataGridView dataGridView;
        BotState botState = BotState.wait;
        long admin_id = 618280964;
        public TeleClass(TextBox tBox, DataGridView dataGrid)
        {
            textBox = tBox;
            dataGridView = dataGrid;
            client = new WebClient();
            Task.Run(() => { UpdateAll(); });
        }
        private void UpdateAll()
        {
            while (true)
            {
                dataGridView.BeginInvoke(new Action(() => UpdateProduct()));
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
            string answer = "", replyMarkup = "";
            string[] _data = item.callback_query.data.Split();
            textBox.BeginInvoke(new Action(() => { WriteLog(item.callback_query.from.first_name + "(" + item.callback_query.message.chat.id + "): " + item.callback_query.data); }));
            switch (_data[0])
            {
                case "/categories":
                    answer = "Выберите категорию:";
                    ShopMenu(out replyMarkup);
                    break;
                case "/cart":
                    answer = ShowCart(item, out replyMarkup);
                    break;
                default:
                    answer = Shop(
                         _data.Length < 1 ? "" : _data[0],
                         _data.Length < 2 ? "" : _data[1],
                         item.callback_query.from.id.ToString(),
                         out replyMarkup);
                    break;
            }
            ChangeMessage(item, answer, replyMarkup);
        }
        public string GetClientProductList(string chat_id)
        {
            string product_list = "";
            using (StorageContext context = new StorageContext())
            {
                ShopCart shopCart = new ShopCart(chat_id);
                var products = context.Carts.Select(t => new { t.UserName, t.Category.CategoryName, t.Product.Name, t.Product.Price }).ToList();
                var prodInfo = products.Where(t => t.UserName == shopCart.user).Select(y => new { y.CategoryName, y.Name, y.Price }).GroupBy(t => new { t.CategoryName, t.Name, t.Price }).Select(t => new { CategoryName = t.Key.CategoryName, ProdName = t.Key.Name, Count = t.Count(), Summary = t.Sum(p => p.Price), Price = t.Key.Price }).ToList();
                foreach (var pr in prodInfo)
                    product_list += $"{pr.ProdName}: {pr.Count}\n";
                double price = Math.Round(prodInfo.Sum(t => t.Summary), 2);
                product_list += $"Сумма: {price}";
            }
            return product_list;
        }
        private void AnswerIsMessage(Result item)
        {
            if(item.callback_query != null)
                textBox.BeginInvoke(new Action(() => { WriteLog(item.callback_query.from.first_name + "(" + item.callback_query.message.chat.id + "): " + item.callback_query.data); }));
            if (item.message.text != null)
                SendAnswer(item.message.chat.id, item.message.text);
            if (item.message.contact != null)
            {
                string product_list = GetClientProductList(item.message.from.id.ToString());
                SendMessageToAdmin($"{item.message.contact.first_name}({item.message.contact.user_id})\n" + $"{item.message.contact.phone_number}\n ждет звонка:\n {product_list}");
                SendMessage(item.message.chat.id, "Ваши контактные данные отправлены. Ждите звонка.\nСпасибо за заказ!", KillButtons());
                ShopCart shopCart = new ShopCart(item.message.from.id.ToString());
                shopCart.DeleteAllProdCart();
                SendAnswer(item.message.chat.id, "/start");
            }
                
        }
        private string KillButtons() => JsonSerializer.Serialize(new RemoveButtons());
        private void SendMessageToAdmin(string message)
        {
            SendMessage(admin_id, message);
        }
        private void WriteLog(string str) => textBox.Text += str + Environment.NewLine;
        private void SendAnswer(long chat_id, string message)
        {
            string answer;
            string replyMarkup = "";
            if (message == null) return;
            if (botState == BotState.send)
            {
                SendMessageToAdmin($"{chat_id}\nАдрес: {message}:\n{GetClientProductList(chat_id.ToString())}");
                ShopMenu(out replyMarkup);
                answer = "Заказ оформлен!";
                ShopCart shopCart = new ShopCart(chat_id.ToString());
                shopCart.DeleteAllProdCart();
                botState = BotState.wait;
                replyMarkup = KillButtons();
                SendMessage(chat_id, answer, replyMarkup);
                SendAnswer(chat_id, "/start");
            }
            else
            {
                switch (message.ToLower())
                {
                    case "/start":
                        answer = ShopMenu(out replyMarkup);
                        break;
                    case "/help":
                        answer = "/help - список комманд\n/start - начать работу с ботом";
                        break;
                    case "написать адрес":
                        botState = BotState.send;
                        answer = "Укажите адрес:";
                        break;
                    default:
                        answer = $"\"{message}\" является неизвестной коммандой!";
                        ShopMenu(out replyMarkup);
                        break;
                }
                SendMessage(chat_id, answer, replyMarkup);
            }
        }
        //private void AnswerToQuery(Result item)
        //{
        //    string address = Url + "/answerCallbackQuery";
        //    NameValueCollection nvc = new NameValueCollection();
        //    nvc.Add("callback_query_id", item.callback_query.id);
        //    nvc.Add("text", "Ага");
        //    client.UploadValues(address, nvc);
        //}
        private string ShopMenu(out string replyMarkup)
        {
            using (StorageContext context = new StorageContext())
            {
                context.Categories.Load();
                var categories = context.Categories.Select(t => new { t.CategoryName }).Distinct().ToList();
                InlineKeyboard keyboard = new InlineKeyboard();
                int i = 0;
                foreach (var category in categories)
                    keyboard.AddButton(new InlineKeyboardButton(category.CategoryName.ToString()), i++ / 2);
                AddAdditionalButtonsMenu(keyboard);
                replyMarkup = JsonSerializer.Serialize(keyboard);
                return "Категории:";
            }
        }
        private void AddAdditionalButtonsMenu(InlineKeyboard keyboard)
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>() 
            {
                new InlineKeyboardButton($"Корзина","/cart show")
            };
            keyboard.AddLineButtons(buttons);
        }
        private void ChangeMessage(Result result, string message, string replyMarkup = "")
        {
            string address = Url + "/editMessageText";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("chat_id", result.callback_query.message.chat.id.ToString());
            nvc.Add("message_id", result.callback_query.message.message_id.ToString());
            nvc.Add("text", message);
            nvc.Add("parse_mode", "HTML");
            if (replyMarkup != null)
                nvc.Add("reply_markup", replyMarkup);
            try { client.UploadValues(address, nvc); }
            catch(Exception e) { }
        }
        private void SendMessage(long chat_id, string message, string replyMarkup = "")
        {
            string address = Url + "/sendMessage";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("chat_id", chat_id.ToString());
            nvc.Add("text", message);
            nvc.Add("parse_mode", "HTML");
            if (replyMarkup != null)
                nvc.Add("reply_markup", replyMarkup);
            client.UploadValues(address, nvc);
        }
        public void UpdateProduct()
        {
            try
            {
                using (StorageContext context = new StorageContext())
                {
                    dataGridView.DataSource = null;
                    var products = context.Products.Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Units,
                        t.Price,
                        t.Quantity,
                        t.Category.CategoryName,
                        t.Description
                    }).ToList();

                    dataGridView.DataSource = products;
                    dataGridView.Columns[0].Visible = false;
                }
            }
            catch(Exception e) { }
        }
        private string Shop(string cat_name, string name, string user, out string replyMarkup)
        {
            string answer = "";
            replyMarkup = "";
            using (StorageContext context = new StorageContext())
            {
                context.Products.Load();
                context.Carts.Load();
                var products = context.Products.Where(t => t.Category.CategoryName == cat_name).Select(t => new { t.Name, t.Category.CategoryName, t.Quantity }).ToList(); 
                if (products.Count == 0) return "Товаров нет";
                InlineKeyboard keyboard = new InlineKeyboard();
                keyboard.AddButton(new InlineKeyboardButton("Назад", "/categories"), 0);
                int i = 0;
                foreach (var product in products)
                {
                    if (name == product.Name.ToString() || product.Quantity == 0) continue;
                    keyboard.AddButton(new InlineKeyboardButton(product.Name.ToString(), product.CategoryName.ToString() + " " + product.Name.ToString()), 1 + i++ / 2);
                }
                AddCart(keyboard, cat_name, name, user);
                replyMarkup = keyboard.ReturnReplymarkup();
                if (name != "")
                {
                    var prod = context.Products.Where(t => t.Category.CategoryName == cat_name).Where(t => t.Name == name).Select(t => new { t.Name, t.Category.CategoryName, t.Price, t.Description, t.Quantity }).ToList();
                    answer = $"{prod[0].Name.ToString()}\nОсталось {prod[0].Quantity} шт.\n{prod[0].Price.ToString()} грн.\n{prod[0].Description}";
                }
                else
                    answer = "Выберите товар:";
            }
            return answer;
        }
        private void AddCart(InlineKeyboard keyboard, string cat_name, string prod_name, string user)
        {
            ShopCart shopCart = new ShopCart(user);
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
            if (shopCart.Count > 0) 
                buttons.Add(new InlineKeyboardButton($"Корзина(всех товаров {shopCart.Count} шт.)", "/cart show"));
            using(StorageContext context = new StorageContext())
            {
                var a = context.Products.Where(t => t.Category.CategoryName == cat_name && t.Name == prod_name).FirstOrDefault();
                if(a != null)
                    if (prod_name != "" && a.Quantity > 0)
                        buttons.Add(new InlineKeyboardButton("Добавить в корзину", "/cart plus_stay " + cat_name + " " + prod_name));
            }
            
            
            keyboard.AddLineButtons(buttons);

        }
        private string ShowCartProducts(ShopCart shopCart, out string answer)
        {
            InlineKeyboard keyboard = new InlineKeyboard();
            using (StorageContext context = new StorageContext())
            {
                var products = context.Carts.Select(t => new { t.UserName, t.Category.CategoryName, t.Product.Name, t.Product.Price }).ToList();
                var prodInfo = products.Where(t => t.UserName == shopCart.user).Select(y => new { y.CategoryName, y.Name, y.Price }).GroupBy(t => new { t.CategoryName, t.Name, t.Price }).Select(t => new { CategoryName = t.Key.CategoryName, ProdName = t.Key.Name, Count = t.Count(), Summary = t.Sum(p => p.Price), Price = t.Key.Price }).ToList();
                int i = 0;
                if(prodInfo.Count > 0)
                {
                    foreach (var pr in prodInfo)
                    {
                        i++;
                        keyboard.AddButton(new InlineKeyboardButton($"{pr.ProdName}: {pr.Count} шт. x {pr.Price}  грн.\n\n", $"/cart plus " + pr.CategoryName + " " + pr.ProdName), i);
                        i++;
                        keyboard.AddButton(new InlineKeyboardButton("+", $"/cart plus " + pr.CategoryName + " " + pr.ProdName), i);
                        keyboard.AddButton(new InlineKeyboardButton("-", $"/cart minus " + pr.CategoryName + " " + pr.ProdName), i);
                    }
                    List<InlineKeyboardButton> list = new List<InlineKeyboardButton>()
                        {
                            new InlineKeyboardButton($"Оформить({Math.Round(prodInfo.Sum(t => t.Summary), 2)} грн.)","/cart buy")
                        };
                    keyboard.AddLineButtons(list);
                    answer = "Ваш заказ:";
                }
                else
                    answer = "Корзина пуста. Быстрее заполните её!";
            }
            keyboard.AddLineButtons(new List<InlineKeyboardButton>() { new InlineKeyboardButton("Назад", "/categories") });
            return keyboard.ReturnReplymarkup();
        }
        private string ShowCart(Result item, out string replyMarkup)
        {
            string answer = "Ваш заказ:";
            replyMarkup = "";
            ShopCart shopCart = new ShopCart(item.callback_query.from.id.ToString());
            string[] _data = item.callback_query.data.Split();
            switch (_data[1] ?? "")
            {
                case "buy":
                    answer = "Выберите метод: отправить номер телефона или написать адрес";
                    replyMarkup = ReplyToAdmin();
                    SendMessage(item.callback_query.message.chat.id, answer, replyMarkup);
                    return "Отмена";
                case "plus":
                    shopCart.AddProdCart(_data[2], _data[3]);
                    replyMarkup = ShowCartProducts(shopCart, out answer);
                    break;
                case "plus_stay":
                    shopCart.AddProdCart(_data[2], _data[3]);
                    answer = Shop(_data[2], _data[3], item.callback_query.from.id.ToString(), out replyMarkup);
                    break;
                case "minus":
                    shopCart.DeleteProdCart(_data[2], _data[3]);
                    replyMarkup = ShowCartProducts(shopCart, out answer);
                    break;
                case "show":
                    replyMarkup = ShowCartProducts(shopCart, out answer);
                    break;
                default:
                    break;
            }
            return answer;
        }
        private string ReplyToAdmin()
        {
            List<KeyboardButton> keyboardButtons = new List<KeyboardButton>()
            {
                new KeyboardButton("Отправить номер телефона",true),
                new KeyboardButton("Написать адрес")
            };
            List<List<KeyboardButton>> keyboards = new List<List<KeyboardButton>>() { keyboardButtons };
            TeleButtons buttons = new TeleButtons(keyboards);
            return buttons.ReturnReplymarkup();
        }
    }
}
