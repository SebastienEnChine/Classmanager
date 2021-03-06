﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MySelector
{
    /// <inheritdoc />
    /// <summary>
    /// 选择器异常类
    /// </summary>
    public class SelectorException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SelectorException()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常信息</param>
        public SelectorException(string message)
            : base(message)
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="innerException">源异常</param>
        public SelectorException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }

    /// <summary>
    /// 选择器类
    /// </summary>
    /// <typeparam name="T">选项类型</typeparam>
    public sealed class Selector<T>
    {
        /// <summary>
        /// 背景颜色(未选中项)
        /// </summary>
        public ConsoleColor UnselectedBackground { get; set; } = ConsoleColor.Black;
        /// <summary>
        /// 前景颜色(未选中项)
        /// </summary>
        public ConsoleColor UnselectedForeground { get; set; } = ConsoleColor.White;
        /// <summary>
        /// 背景颜色(选中项)
        /// </summary>
        public ConsoleColor SelectedBackground { get; set; } = ConsoleColor.White;
        /// <summary>
        /// 前景颜色(选中项)
        /// </summary>
        public ConsoleColor SelectedForeground { get; set; } = ConsoleColor.Black;
        /// <summary>
        /// 选项
        /// </summary>
        public ImmutableList<T> Select { get; set; }
        /// <summary>
        /// 选项显示信息
        /// </summary>
        public ImmutableList<string> TheInfomationOfSelect { get; set; }
        /// <summary>
        /// 选中项索引
        /// </summary>
        private int _mainIndex;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="info">选项显示信息</param>
        /// <param name="select">选项</param>
        ///<exception cref="NullReferenceException">info或select为null时引发此异常</exception>
        ///<exception cref="SelectorException">info和select长度不匹配时引发此异常</exception>
        public Selector(List<string> info, params T[] select)
        {
            if (info == null || select == null)
            {
                throw new NullReferenceException();
            }
            if (info.Count != select.Length)
            {
                throw new SelectorException();
            }
            this.Select = select.ToImmutableList();
            this.TheInfomationOfSelect = info.ToImmutableList();
        }

        /// <summary>
        /// 显示选项信息并获取用户的选择
        /// </summary>
        /// <returns>用户的选择结果</returns>
        public T GetSelect()
        {
            //保存控制台原有颜色
            ConsoleColor oldbg = Console.BackgroundColor;
            ConsoleColor oldfg = Console.ForegroundColor;

            do
            {
                DisplayTheInfomationOfSelect();
                ConsoleKeyInfo info = Console.ReadKey(true);
                switch (info.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (this._mainIndex > 0)
                        {
                            --this._mainIndex;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (this._mainIndex < this.Select.Count - 1)
                        {
                            ++this._mainIndex;
                        }
                        break;
                    case ConsoleKey.Enter:
                        //恢复控制台原有颜色
                        Console.BackgroundColor = oldbg;
                        Console.ForegroundColor = oldfg;

                        return this.Select[this._mainIndex];
                    default:
                        break;
                }

                Console.SetCursorPosition(0, Console.CursorTop - this.Select.Count);
            } while (true);
        }
        /// <summary>
        /// 显示选项信息
        /// </summary>
        private void DisplayTheInfomationOfSelect()
        {
            for (int index = 0; index < this.TheInfomationOfSelect.Count; ++index)
            {
                if (this._mainIndex == index)
                {
                    SetSelectColor();
                    Console.Write($"{"",-10}{this.TheInfomationOfSelect[index]}");
                    SetColor();
                    Console.WriteLine(".");
                }
                else
                {
                    Console.WriteLine($"{" ",-10}{this.TheInfomationOfSelect[index]}");
                }
            }

            //默认颜色设置
            void SetColor()
            {
                Console.BackgroundColor = this.UnselectedBackground;
                Console.ForegroundColor = this.UnselectedForeground;
            }
            //焦点颜色设置
            void SetSelectColor()
            {
                Console.BackgroundColor = this.SelectedBackground;
                Console.ForegroundColor = this.SelectedForeground;
            }
        }
    }
}
