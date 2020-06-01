using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using TTMLibrary.Models;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using MaterialDesignColors;

namespace TTMClient.Models
{
    public class GroupModel : ObservableObject
    {
        private Group group;
        private MessageModel lastMessage;
        private string imagePatch;

        public Guid Id { get => group.Id; set => group.Id = value; }
        public string Name { get => group.Name; set => group.Name = value; }
        public string CreatorLogin { get => group.CreatorId; set => group.CreatorId = value; }
        //public Color? Color { get => group.Color != string.Empty ? (Color)ColorConverter.ConvertFromString(group.Color) : default; set => group.Color = value?.ToString(); }
        public string Color { get => group.Color; set => group.Color = value; }
        public char FirstSymbol { get => Name.ToUpper()[0]; }
        public string CharColor
        {
            get
            {
                Color color = ColorTranslator.FromHtml(Color);
                Color newColor = System.Drawing.Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
                return ColorTranslator.ToHtml(newColor);
            }
        }
        public uint NewMessagesCount { get; set; }
        public string ImagePatch //используется при создании чата
        {
            get => imagePatch;
            set
            {
                imagePatch = value;
                RaisePropertyChanged();
            }
        }
        public MessageModel LastMessage
        {
            get => lastMessage;
            set
            {
                lastMessage = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<MessageModel> Messages { get; set; }
        public GroupModel(object groupObj)
        {
            Messages = new ObservableCollection<MessageModel>();
            group = groupObj as Group;

            if (group == null)
            {
                throw new NullReferenceException();
            }



        }

    }
}
