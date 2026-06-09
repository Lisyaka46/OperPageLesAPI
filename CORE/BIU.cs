using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;
using WnColor = System.Windows.Media.Color;

namespace OPLAPI.CORE
{
    /// <summary>
    /// Обработчик строки форматированного текста
    /// </summary>
    public partial class BIU
    {
        #region ManipulateText
        /// <summary>
        /// Изменить формативность текста с учётом первых знаков
        /// </summary>
        /// <remarks>
        /// %#FFFFFF** <b>Italic</b> **<br/>
        /// <br/>
        /// ** <b>Bold</b> **<br/>
        /// // <i>Italic</i> //<br/>
        /// __ <u>UnderLine</u> __<br/>
        /// </remarks>
        /// <param name="Text">Текст форматирования</param>
        /// <returns>Форматированный текст</returns>
        public static Span FormattedAllTextDetect(string Text)
        {
            // %//Italic %**Bold**//
            Span Result = new();
            foreach (Match match in RegexFormattedText().Matches(Text))
            {
                Result.Inlines.AddRange(FormattedBlockText(match.Value));
            }
            return Result;
        }

        private static Inline[] FormattedBlockText(string Text)
        {
            Span Result = new();
            if (Text.Length < 2 || Text[0] != '%')
            {
                Result.Inlines.Add(Text);
                return [.. Result.Inlines];
            }

            Text = Text[1..]; // удаление "%"

            // логика цвета
            SolidColorBrush? BackgroundColor = null;
            if (Text[0] == '#')
            {
                BackgroundColor = new((WnColor)System.Windows.Media.ColorConverter.ConvertFromString(
                    RegexFormattedTextColor().Match(Text).Value));
                Text = Text[7..];
            }

            MatchCollection CollectionRecurce = RegexFormattedText().Matches(Text[2..^2]);
            foreach (Match match in CollectionRecurce)
            {
                if (match.Value[0] == '%' && match.Value.Length > 1)
                {
                    foreach (Inline Element in FormattedBlockText(match.Value))
                    {
                        Result.Inlines.Add(SwitchBlockText([Text[0], Text[1]], Element));
                        Result.Inlines.LastInline.Background = BackgroundColor;
                    }
                    continue;
                }
                else
                    Result.Inlines.Add(SwitchBlockText([Text[0], Text[1]], new Run(match.Value)));
                Result.Inlines.LastInline.Background = BackgroundColor;
            }
            return [.. Result.Inlines];
        }

        private static Inline SwitchBlockText(char[] Parrent, Inline Context)
        {
            Contract.Requires(Parrent.Length == 2);
            return string.Concat(Parrent) switch
            {
                "**" => new Bold(Context),
                "//" => new Italic(Context),
                "__" => new Underline(Context),
                _ => Context,
            };
        }
        #endregion

        #region Regex
        /// <summary>
        /// Регулярное выражение сортировки параметров от специальных символов
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        // Текст который является %#00FF00FF__%**регистрационным**__ и %#FFFFFF**может** %~~даже так~~ %--постоянно-- %__форматироваться__
        [GeneratedRegex(@"([^%]+|(\%(#[0-9A-F]{6})?)(\*{2}([^\*]+(\*{3,}|\*)){1,}\*|_{2}([^_]+(_{3,}|_)){1,}_|\/{2}([^\/]+(\/{3,}|\/)){1,}\/)|\%)")]
        private static partial Regex RegexFormattedText();

        /// <summary>
        /// Регулярное выражение сортировки параметров от специальных символов
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        // %   #FFFFFF   //**d**//
        [GeneratedRegex(@"#[0-9A-F]{6}")]
        private static partial Regex RegexFormattedTextColor();
        #endregion
    }
}