using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram_Bot_Practice.Models;
using Telegram_Bot_Practice.Services;
using Telegram_Bot_Practice.Utilities;

namespace Telegram_Bot_Practice.Controllers
{
    public class TextMessageController
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly IStorage _memoryStorage;

        public TextMessageController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            switch (message.Text)
            {
                case "/start":
                    DisplayMenu(message, ct);
                    break;
                default:
                    switch (_memoryStorage.GetSession(message.Chat.Id)._func)
                    {
                        case "sum":
                            try
                            {
                                checked
                                {
                                    int Sum = Summator.TrySum(message.Text);
                                }
                            }
                            catch (FormatException ex)
                            {
                                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Введенные данные не являются числами", parseMode: ParseMode.Html);
                                return;
                            }
                            catch (OverflowException ex)
                            {
                                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Итоговое значение слишком велико или слишком мало", parseMode: ParseMode.Html);
                                return;
                            }
                            catch (Exception ex)
                            {
                                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Произошла непридвиденная ошибка при сложении чисел", parseMode: ParseMode.Html);
                                return;
                            }
                            await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Сумма чисел: {Summator.TrySum(message.Text).ToString()}", parseMode: ParseMode.Html);
                            break;
                        case "charCount":
                            await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Количество символов: {CharCounter.Count(message.Text).ToString()}", parseMode: ParseMode.Html);
                            break;
                        default:
                            await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Сначала необходимо выбрать функцию.", parseMode: ParseMode.Html);
                            DisplayMenu(message, ct);
                            break;
                    }
                    break;
            }
        }

        public async void DisplayMenu(Message message, CancellationToken ct)
        {
            // Объект, представляющий кноки
            var buttons = new List<InlineKeyboardButton[]>();
            buttons.Add(new[]
            {
                        InlineKeyboardButton.WithCallbackData($" Сумма" , $"sum"),
                        InlineKeyboardButton.WithCallbackData($" Количество символов" , $"charCount")
            });

            // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
            await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"<b>  Наш бот может просуммировать числа или посчитать количество символов.</b> {Environment.NewLine}" +
                $"{Environment.NewLine}Выберете функцию, которая вам необходима.{Environment.NewLine}", cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));
        }
    }
}
