using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLibrary
{
    public class TeleButtons
    {
        public List<List<string>> keyboard { get; set; }
        public bool one_time_keyboard { get; set; }
        public TeleButtons(List<List<string>> keyboard, bool one_time_keyboard = false)
        {
            this.keyboard = keyboard;
            this.one_time_keyboard = one_time_keyboard;
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
        public InlineKeyboardButton(string _text, string _callback_data)
        {
            this.text = _text;
            this.callback_data = _callback_data == "" ? _text : _callback_data;
        }
    }
    public class InlineKeyboard
    {
        public List<List<InlineKeyboardButton>> inline_keyboard { get; set; }
        public InlineKeyboard(List<List<InlineKeyboardButton>> inline_keyboard)
        {
            this.inline_keyboard = inline_keyboard;
        }
    }
    public class Inline_keyboardItem
    {
        public string text { get; set; }
        public string callback_data { get; set; }
    }
}
