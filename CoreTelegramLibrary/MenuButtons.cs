using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreTelegramLibrary
{
    public class TeleButtons
    {
        public List<List<KeyboardButton>> keyboard { get; set; }
        public bool one_time_keyboard { get; set; }
        public TeleButtons(List<List<KeyboardButton>> keyboard, bool one_time_keyboard = true)
        {
            this.keyboard = keyboard;
            this.one_time_keyboard = one_time_keyboard;
        }
        public string ReturnReplymarkup()
        {
            return JsonSerializer.Serialize(this);
        }
    }
    public class RemoveButtons
    {
        public bool remove_keyboard { get; set; }
        public RemoveButtons()
        {
            remove_keyboard = true;
        }
    }
    public class InlineKeyboardButton
    {
        public string text { get; set; }
        public string callback_data { get; set; }
        public InlineKeyboardButton(string _text, string _callback_data = "")
        {
            text = _text;
            callback_data = _callback_data == "" ? _text : _callback_data;
        }
    }
    public class InlineKeyboard
    {
        public List<List<InlineKeyboardButton>> inline_keyboard { get; set; }
        public InlineKeyboard(List<List<InlineKeyboardButton>> inline_keyboard) => this.inline_keyboard = inline_keyboard;
        public InlineKeyboard() => inline_keyboard = new List<List<InlineKeyboardButton>>();
        public void AddButton(InlineKeyboardButton button, int numLine)
        {
            while (inline_keyboard.Count <= numLine)
                inline_keyboard.Add(new List<InlineKeyboardButton>());
            inline_keyboard[numLine].Add(button);
        }
        public void AddLineButtons(List<InlineKeyboardButton> line) => inline_keyboard.Add(line);
        public string ReturnReplymarkup()
        {
            return JsonSerializer.Serialize(this);
        }
    }
    public class KeyboardButton
    {
        public string text { get; set; }
        public bool request_contact { get; set; }
        public bool request_location { get; set; }
        public KeyboardButton(string text, bool request_contact = false, bool request_location = false)
        {
            this.text = text;
            this.request_contact = request_contact;
            this.request_location = request_location;
        }
    }
    public class Inline_keyboardItem
    {
        public string text { get; set; }
        public string callback_data { get; set; }
    }
}
