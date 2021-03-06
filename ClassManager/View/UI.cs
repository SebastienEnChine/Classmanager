﻿using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using static System.Console;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sebastien.ClassManager.Enums;

namespace Sebastien.ClassManager.Core
{
    /// <summary>
    /// 控制台应用程序界面
    /// </summary>
    public static class Ui
    {
        /// <summary>
        /// 同步锁
        /// </summary>
        private static readonly object LocalLock = new object();
        /// <summary>
        /// 任务取消令牌
        /// </summary>
        //public static CancellationTokenSource _cts = new CancellationTokenSource();

        /// <summary>
        /// 获取Subject类型的成员个数
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public static int GetLengthOfSubject(this Subject subject)
        {
            return Enum.GetNames(subject.GetType()).Length;
        }

        #region 用户，学生，教师， 班主任类的扩展方法
        /// <summary>
        /// 随机向Information.StudentLibrary几何中添加100个Student对象
        /// Todo: 可修改为使用多线程操作, 并使用并发集合或线程同步
        /// </summary>
        public static async void AddStudents()
        {
            await Task.Run(() =>
            {
                for (int index = 0; index < 100; ++index)
                {
                    UserRepository.StudentLibrary.Add(new Student($"index{index}", "1234", $"index{index}"));
                }
            });
        }
        /// <summary>
        /// 获取帮助(指定用户)
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static void GetHelp(this UserCore me)
        {
            switch (me.UserType)
            {
                case Identity.Student:
                    GetHelpForStudent();
                    break;
                case Identity.Instructor:
                    GetHelpForInstructor();
                    break;
                case Identity.HeadTeacher:
                    GetHelpForHeadTeacher();
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        /// <summary>
        /// 获取基本帮助
        /// </summary>
        private static void GetHelpForUser()
        {
            WriteLine("SwitchUser: 切换用户");
            WriteLine("ChangePasswd: 修改密码");
            WriteLine("ChangeAge: 修改年龄");
            WriteLine("ChangeAddress: 修改地址");
            WriteLine("ChangeSex: 修改性别");
            WriteLine("ShowMe: 个人信息概览");
            WriteLine("StudentsPreview: 学生列表预览");
            WriteLine("TeachersPreview: 教师列表预览");
            WriteLine("ViewCurriculums: 查看课表");
            WriteLine("ViewMyHistory: 查看我的操作记录");
            WriteLine("LeaveAMessage: 在公共留言墙上写一条公共留言");
            WriteLine("ViewLeaveMessages: 浏览公共留言墙");
            WriteLine("UrlTest: 测试此计算机能否被指定Web地址响应");
            WriteLine("Exit: 退出程序");
        }
        /// <summary>
        /// 获取帮助(学生用户)
        /// </summary>
        private static void GetHelpForStudent()
        {
            GetHelpForUser();
            WriteLine("MyScore: 查看各科目成绩");
            WriteLine("ViewNews: 查看我收到的新消息");
            WriteLine("ViewAllNews: 查看消息记录");
            WriteLine("SubscriptionToHeadTeacher: 订阅班主任");
            WriteLine("UnsubscribeToHeadTeacher: 取消订阅班主任");
        }
        /// <summary>
        /// 获取帮助(教师用户)
        /// </summary>
        private static void GetHelpForInstructor()
        {
            GetHelpForUser();
            WriteLine("AllSocre: 显示本班学生的本科目成绩(不排序)");
            WriteLine("AllSocreAndRank: 显示本班学生的本科目成绩(排序)");
            WriteLine("ChangeScore: 设置或修改学生的成绩");
            WriteLine("HighThan: 查看高于指定分数的所有学生");
            WriteLine("ReleaseAMsg: 广播一条消息");
        }
        /// <summary>
        /// 获取帮助(班主任用户)
        /// </summary>
        private static void GetHelpForHeadTeacher()
        {
            GetHelpForUser();
            WriteLine("AddStudent: 新生注册");
            WriteLine("AddTeacher: 新老师注册");
            WriteLine("Remove: 永久删除一位学生或老师账号");
            WriteLine("AllSocre: 显示本班学生的成绩(不排序)");
            WriteLine("AllSocreAndRank: 显示本班学生的成绩(排序)");
            WriteLine("HighThan: 查看总分高于指定分数的所有学生");
            WriteLine("ChangeName: 修改姓名");
            WriteLine("ReleaseNewCurriculum: 发布新课表");
            WriteLine("ReleaseAMsg: 广播一条消息");
        }

        /// <summary>
        /// 用户选择界面 Todo: 未实现
        /// </summary>
        /// <returns></returns>
        public static void TheSelectOfUser()
        {
            BackgroundColor = ConsoleColor.Green;
            ForegroundColor = ConsoleColor.White;
            Clear();
            SetCursorPosition(50, 10);
            WriteLine("Who are you?");
            dynamic dm = Client.GetSelectorObject<Identity>(
                info: new List<string>
                {
                    "Student",
                    "Teacher",
                    "HeadTeacher"
                },
                selects: new[]
                {
                    Identity.Student,
                    Identity.Instructor,
                    Identity.HeadTeacher
                }
                );
            dm.UnselectedBackground = BackgroundColor;
            dm.UnselectedForeground = ForegroundColor;

            Identity result = dm.GetSelect();
            Clear();
            WriteLine($"your selector: {result}");
            ReadKey();
        }
        /// <summary>
        /// 显示命令提示符
        /// </summary>
        /// <param name="me"></param>
        public static void Prompt(this UserCore me)
        {
            switch (me.UserType)
            {
                case Identity.Student:
                    ForegroundColor = ConsoleColor.Cyan;
                    break;
                case Identity.Instructor:
                    ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case Identity.HeadTeacher:
                    ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Write($"[ {DateTime.Now.ToLongTimeString()} ] {me.UserType}@{me.Name}$> ");
            DefaultColor();
        }
        /// <summary>
        /// 显示登录成功问候语
        /// </summary>
        /// <param name="me">当前用户</param>
        public static void SayHello(this UserCore me)
        {
            switch (DateTime.Now.Hour)
            {
                case 7:
                case 8:
                case 9:
                    WriteLine($"早上好，{me.Name}~");
                    break;
                case 10:
                case 11:
                case 12:
                    WriteLine($"上午好，{me.Name}~");
                    break;
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    WriteLine($"下午好，{me.Name}~");
                    break;
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                    WriteLine($"晚上好，{me.Name}~");
                    break;
                default:
                    WriteLine($"晚上好，{me.Name}~");
                    WriteLine("天很晚了，早点休息");
                    break;
            }
        }

        /// <summary>
        /// 显示学生列表
        /// </summary>
        public static void DisplayStudentList()
        {
            if (UserRepository.StudentLibrary.Count == 0)
            {
                DisplayTheInformationOfErrorCode(ErrorCode.NoDisplayableInformation);
                return;
            }

            WriteLine($"{"Name",-10}{"Account",-10}{"UserType",-10}{"Sex",-7}{"Age",-5}{"CreatedTime",-20}");
            bool isWhite = true;
            Parallel.ForEach(UserRepository.StudentLibrary, stu =>
            {
                bool lockTaken = false;                //必须为false
                Monitor.TryEnter(LocalLock, 100, ref lockTaken);
                if (lockTaken)
                {
                    try
                    {
                        ConsoleColor bg = isWhite ? ConsoleColor.White : ConsoleColor.Black;
                        ConsoleColor fg = isWhite ? ConsoleColor.Black : ConsoleColor.White;
                        PrintColorMsg($"{stu.Name,-10}", bg, ConsoleColor.DarkYellow);
                        PrintColorMsg($"{stu.Account,-10}{stu.UserType,-10}{stu.Sex,-7}{stu.Age,-5}{stu.CreatedTime,-20}", bg, fg);
                        WriteLine();
                        isWhite = !isWhite;
                    }
                    finally
                    {
                        Monitor.Exit(LocalLock);
                    }
                }
                else
                {
                    WriteLine("获取锁超时");
                }
            });
        }

        /// <summary>
        /// 显示教师列表
        /// </summary>
        /// <param name="me"></param>
        public static void DisplayTeacherList(this UserCore me)
        {
            if (UserRepository.TeacherLibrary.Count > 0)
            {
                var sLock = new SpinLock();
                bool isWhite = true;
                WriteLine($"{"Name",-10}{"Account",-10}{"UserType",-10}{"Sex",-7}{"Age",-5}{"CreatedTime",-20}{"Since",-7}");
                Parallel.For(0, UserRepository.TeacherLibrary.Count, index =>
                {
                    bool hasLock = false;
                    sLock.Enter(ref hasLock);
                    if (hasLock)
                    {
                        try
                        {
                            ConsoleColor bg = isWhite ? ConsoleColor.White : ConsoleColor.Black;
                            ConsoleColor fg = isWhite ? ConsoleColor.Black : ConsoleColor.White;
                            PrintColorMsg($"{UserRepository.TeacherLibrary[index].Name,-10}",
                                                    bg, ConsoleColor.DarkYellow);
                            PrintColorMsg($"{UserRepository.TeacherLibrary[index].Account,-10}" +
                                                    $"{UserRepository.TeacherLibrary[index].UserType,-10}" +
                                                    $"{UserRepository.TeacherLibrary[index].Sex,-7}" +
                                                    $"{UserRepository.TeacherLibrary[index].Age,-5}" +
                                                    $"{UserRepository.TeacherLibrary[index].CreatedTime,-20}" +
                                                    $"{UserRepository.TeacherLibrary[index].YearsOfProfessional,-7}",
                                                    bg, fg);
                            WriteLine();
                            isWhite = !isWhite;
                        }
                        finally
                        {
                            sLock.Exit();
                        }
                    }
                    else
                    {
                        WriteLine("获取锁超时");
                    }
                });
            }
            else
            {
                DisplayTheInformationOfErrorCode(ErrorCode.NoDisplayableInformation);
            }
        }
        /// <summary>
        /// 修改密码(调用和异常处理)
        /// </summary>
        /// <param name="me">当前用户</param>
        public static void CallChangeMyPasswd(this UserCore me)
        {
            try
            {
                me.ChangeMyPasswd();
            }
            catch (FormatException)
            {
                DisplayTheInformationOfErrorCode(ErrorCode.ArgumentError);
            }
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="me">当前用户</param>
        private static void ChangeMyPasswd(this UserCore me)
        {
            Write("请输入当前密码: ");
            string currentPasswd = ReadLine();
            Write("请输入新密码: ");
            string firstPasswd = ReadLine();
            Write("请再次输入新密码: ");
            string secondPasswd = ReadLine();
            if (firstPasswd != null && !firstPasswd.Equals(secondPasswd))
            {
                DisplayTheInformationOfErrorCode(ErrorCode.InconsistentPassword);
            }
            else
            {
                me.Passwd = secondPasswd;
                me.AddHistory(new Message("你", "重新设置了密码"));
                DisplayTheInformationOfSuccessfully();
            }
        }
        /// <summary>
        /// 修改年龄(调用和异常处理)
        /// </summary>
        /// <param name="me">当前用户</param>
        public static void CallChangeMyAge(this UserCore me)
        {
            try
            {
                me.ChangeMyAge();
            }
            catch (ArgumentOutOfRangeException)
            {
                DisplayTheInformationOfErrorCode(ErrorCode.ArgumentOutOfRange);
            }
            catch (ArgumentException)
            {
                DisplayTheInformationOfErrorCode(ErrorCode.ArgumentError);
            }
        }
        /// <summary>
        /// 修改年龄
        /// </summary>
        /// <param name="me">当前用户</param>
        private static void ChangeMyAge(this UserCore me)
        {
            Write("请输入你当前的年龄: ");
            if (int.TryParse(ReadLine(), out int age))
            {
                me.Age = age;
                me.AddHistory(new Message("你", $"重新设置了年龄({me.Age})"));
                DisplayTheInformationOfSuccessfully();
            }
            else
            {
                throw new ArgumentException();
            }
        }
        /// <summary>
        /// 修改地址
        /// </summary>
        /// <param name="me">当前用户</param>
        public static void ChangeMyAddress(this UserCore me)
        {
            Write("地址: ");
            string address = ReadLine();
            me.AddHistory(new Message("你", $"重新设置了地址({me.Address = address})"));
            DisplayTheInformationOfSuccessfully();
        }
        /// <summary>
        /// 修改性别
        /// </summary>
        /// <param name="me">当前用户</param>
        public static void ChangeMySex(this UserCore me)
        {
            WriteLine("设置新性别: (选择: 上/下方向键   确定: 回车键) ");
            dynamic dm = Client.GetSelectorObject(new List<string> { "男", "女" }, TheSex.Male, TheSex.Frame);
            TheSex result = dm.GetSelect();
            //不使用动态加载
            //TheSex result = new Selector<TheSex>(new List<String> { "男", "女" }, TheSex.Male, TheSex.Frame).GetSubject();
            me.AddHistory(new Message("你", $"重新设置了性别({me.Sex = result})"));
            DisplayTheInformationOfSuccessfully();
        }
        /// <summary>
        /// 查看我的操作记录
        /// </summary>
        /// <param name="me">当前用户</param>
        public static void ViewMyHistory(this UserCore me)
        {
            me.GetHistory().OrderByDescending(msg => msg.Time).ToList().ForEach(WriteLine);

            //var messages = (from msg in me.GetHistory()
            //                          orderby msg.Time descending
            //                          select msg).ToList();
            //messages.ForEach(WriteLine);
        }
        /// <summary>
        /// 查看课表
        /// </summary>
        public static void ViewCurriculum(this UserCore me)
        {
            Client.UpdateCurriculum();
            if (UserRepository._curriculums[0] == null)
            {
                DisplayTheInformationOfErrorCode(ErrorCode.NoDisplayableInformation);
            }
            else
            {
                WriteLine($"本周课表：\n于{UserRepository._curriculums[0].OverTime}失效");
                UserRepository._curriculums[0].Draw();
                if (UserRepository._curriculums[1] == null) return;
                WriteLine($"下周课表：\n于{UserRepository._curriculums[1].OverTime}失效");
                UserRepository._curriculums[1].Draw();
            }
        }

        /// <summary>
        /// 新消息提示
        /// </summary>
        /// <param name="stu"></param>
        public static void TheTipsOfNews(this Student stu)
        {
            if (!stu.HasNewMsg) return;
            SetCursorPosition(0, CursorTop);
            PrintColorMsg($"[ {DateTime.Now.ToLongTimeString()} ] {stu.UserType}@{stu.Name}", ConsoleColor.Black, ConsoleColor.Cyan);
            PrintColorMsg("[新消息]", ConsoleColor.Black, ConsoleColor.DarkMagenta);
            PrintColorMsg(">", ConsoleColor.Black, ConsoleColor.Cyan);
        }

        /// <summary>
        /// 显示成绩列表
        /// </summary>
        /// <param name="teacher"></param>
        public static void DisplayAllScoreOfStudent(this ITeacher teacher, State IsDisplayRank = State.Off, State IsSort = State.Off)
        {
            if (UserRepository.StudentLibrary.Count <= 0)
            {
                DisplayTheInformationOfErrorCode(ErrorCode.NoDisplayableInformation);
                return;
            }
            DisplayAllScoreTitle(IsSort);
            teacher.ViewScoreOfAllStudent(IsDisplayRank, IsSort);

            //用于显示成绩的列的标题
            void DisplayAllScoreTitle(State isSortAndRank)
            {
                if (isSortAndRank == State.On)
                {
                    Write($"{"No.",-10}");
                }
                WriteLine($"{"Name",-10}{"C",-10}{"Java",-10}{"C++",-10}{"C#",-10}{"Html&Css",-10}{"SQL",-10}{"Total",-10}");
            }
        }
        /// <summary>
        /// 显示高于指定分数的所有学生
        /// </summary>
        /// <param name="teacher"></param>
        public static void DisplayScoreHighThan(this ITeacher teacher)
        {
            try
            {
                Write("你想将分数指定为: ");
                teacher.GetStuHighThan(int.Parse(ReadLine()));
            }
            catch (FormatException)
            {
                DisplayTheInformationOfErrorCode(ErrorCode.ArgumentError);
            }
            catch (ArgumentOutOfRangeException)
            {
                DisplayTheInformationOfErrorCode(ErrorCode.ArgumentOutOfRange);
            }
        }
        /// <summary>
        /// 广播一条消息
        /// </summary>
        /// <param name="teacher"></param>
        public static void ReleaseNewMessage(this Teacher teacher)
        {
            Write("在此处输入将要广播的消息> ");
            var msg = new Message("班主任", ReadLine());
            teacher.ReleaseNewMsg(msg);
            teacher.AddHistory(new Message("你", $"广播了一条消息: {msg.Content}"));
            DisplayTheInformationOfSuccessfully("(广播已发布)");
        }

        /// <summary>
        /// 修改姓名(针对于班主任用户)
        /// </summary>
        public static void ChangeMyName(this HeadTeacher ht)
        {
            Write("新姓名: ");
            string name = ReadLine();

            Debug.Assert(UserRepository.HeadTeacherUser != null, "InformationLibrary.HeadTeacherUser != null");
            UserRepository.HeadTeacherUser.Name = name;

            ht.AddHistory(new Message("你", $"重新设置了你的姓名({name})"));
            DisplayTheInformationOfSuccessfully();
        }
        /// <summary>
        /// 修改学生或教师的姓名(针对于班主任用户)
        /// </summary>
        /// <param name="me">执行该命令的对象</param>
        public static void ChangeNameOfOtherUser(this HeadTeacher ht)
        {
            Write("请输入将要更改姓名的账户：");
            string account = ReadLine();
            foreach (Student student in UserRepository.StudentLibrary)
            {
                if (account != student.Account) continue;
                Write("新姓名: ");
                string name = ReadLine();
                student.Name = name;
                UserRepository.HeadTeacherUser.AddHistory(
                    new Message("你", $"将{student.Account}的姓名重新设置为({name})")
                );
                DisplayTheInformationOfSuccessfully();
                return;
            }
            foreach (Instructor teacher in UserRepository.TeacherLibrary)
            {
                if (account != teacher.Account) continue;
                Write("新姓名: ");
                string name = ReadLine();

                teacher.Name = name;
                UserRepository.HeadTeacherUser.AddHistory(
                    new Message("你", $"将{teacher.Account}的姓名重新设置为({name})")
                );
                DisplayTheInformationOfSuccessfully();
                return;
            }
        }

        /// <summary>
        /// 添加新生
        /// </summary>
        /// <param name="info">所有用户信息库</param>
        public static void CallAddStudent(this HeadTeacher ht)
        {
            try
            {
                ht?.AddStudent();
            }
            catch (NullReferenceException ex) when (ex.Message.Equals("Account is Empty"))
            {
                DisplayTheInformationOfErrorCode(ErrorCode.BadAccount);
            }
            catch (NullReferenceException ex) when (ex.Message.Equals("Passwd is Empty"))
            {
                DisplayTheInformationOfErrorCode(ErrorCode.BadPasswd);
            }
        }
        /// <summary>
        /// 添加新生
        /// </summary>
        /// <param name="info">所有用户信息库</param>
        public static void AddStudent(this HeadTeacher ht)
        {
            Write("账号: ");
            string account = ReadLine();
            if (account.Contains(" ") || account.Equals(string.Empty))
            {
                throw new NullReferenceException("Account is Empty");
            }

            if (Client.CheckAccountAvailability(account) != null)
            {
                DisplayTheInformationOfErrorCode(ErrorCode.AccountAlreadyExists, account);
                return;
            }

            Write("密码: ");
            string passwd = EnterPasswd();
            if (passwd.Contains(" ") || passwd.Equals(string.Empty))
            {
                throw new NullReferenceException("Passwd is Empty");
            }

            Write("姓名: ");
            string name = ReadLine();
            UserRepository.StudentLibrary.Add(new Student(account, passwd, name));
            UserRepository.HeadTeacherUser.AddHistory(new Message("你", $"注册了一个学生账户({account})"));
            ht.ReleaseNewMsg(new Message("班主任", $"班里来了一位新同学({account}), 快去看看吧~"));
            DisplayTheInformationOfSuccessfully();
        }
        /// <summary>
        /// 添加/注册新老师(调用和异常处理)
        /// </summary>
        /// <param name="info">所有用户信息库</param>
        public static void CallAddTeacher(this HeadTeacher ht)
        {
            try
            {
                ht.AddTeacher();
            }
            catch (NullReferenceException ex) when (ex.Message.Equals("Account is Empty"))
            {
                DisplayTheInformationOfErrorCode(ErrorCode.BadAccount);
            }
            catch (NullReferenceException ex) when (ex.Message.Equals("Passwd is Empty"))
            {
                DisplayTheInformationOfErrorCode(ErrorCode.BadAccount);
            }
            catch (ArgumentException)
            {
                DisplayTheInformationOfErrorCode(ErrorCode.ArgumentError);
            }
        }

        /// <summary>
        /// 添加/注册新老师
        /// </summary>
        public static void AddTeacher(this HeadTeacher ht)
        {
            WriteLine("你想创建哪一科目的任课教师? (选择: 上/下方向键   确定: 回车键) ");
            #region 不使用动态加载
            //Subject result = new Selector<Subject>(
            //                              new List<String>()
            //                               {
            //                                    "C语言任课老师",
            //                                    "C++任课老师",
            //                                    "C#任课老师",
            //                                    "HTML/Css任课老师",
            //                                    "Java任课老师",
            //                                    "SQL数据库任课老师"
            //                               },
            //                              new Subject[]
            //                              {
            //                                     Subject.C,
            //                                     Subject.CPlusPlus,
            //                                     Subject.CSharp,
            //                                     Subject.HtmlAndCss,
            //                                     Subject.Java,
            //                                     Subject.SQL
            //                              }
            //                           ).GetSubject();
            #endregion

            dynamic dm = Client.GetSelectorObject(
                                            info: new List<string>
                                            {
                                                "C语言任课老师",
                                                "C++任课老师",
                                                "C#任课老师",
                                                "HTML/Css任课老师",
                                                "Java任课老师",
                                                "SQL数据库任课老师"
                                            },
                                          selects: new[]
                                          {
                                                 Subject.C,
                                                 Subject.CPlusPlus,
                                                 Subject.CSharp,
                                                 Subject.HtmlAndCss,
                                                 Subject.Java,
                                                 Subject.Sql
                                          }) ?? throw new NullReferenceException();
            Subject result = dm.GetSelect();

            Write("账号: ");
            string account = ReadLine();
            if (account.Contains(" ") || account.Equals(string.Empty))
            {
                throw new NullReferenceException("Account is Empty");
            }
            if (Client.CheckAccountAvailability(account) != null)
            {
                DisplayTheInformationOfErrorCode(ErrorCode.AccountAlreadyExists, account);
                return;
            }

            Write("密码: ");
            string passwd = EnterPasswd();
            if (passwd.Contains(" ") || passwd.Equals(string.Empty))
            {
                throw new NullReferenceException("Passwd is Empty");
            }

            Write("姓名: ");
            string name = ReadLine();

            WriteLine("此用户从哪一年开始从事该行业？");
            Write("> ");
            if (!int.TryParse(ReadLine(), out int years))
            {
                throw new ArgumentException();
            }

            UserRepository.TeacherLibrary.Add(new Instructor(account, passwd, name, years, result));
            UserRepository.HeadTeacherUser.AddHistory(new Message("你", $"注册了一个教师账户({account})"));
            ht.ReleaseNewMsg(new Message("班主任", $"班里来了一位新老师({account}), 快去看看吧~"));
            DisplayTheInformationOfSuccessfully();
        }
        /// <summary>
        /// 修改学生的成绩(调用和异常处理)
        /// </summary>
        /// <param name="curr">执行此命令的老师</param>
        public static void CallChangeScore(this Instructor curr)
        {
            try
            {
                curr.ChangeScore();
            }
            catch (NullReferenceException)
            {
                DisplayTheInformationOfErrorCode(ErrorCode.CantFindThisAccount);
            }
            catch (ArgumentOutOfRangeException)
            {
                DisplayTheInformationOfErrorCode(ErrorCode.ArgumentOutOfRange);
            }
            catch (ArgumentException)
            {
                DisplayTheInformationOfErrorCode(ErrorCode.ArgumentOutOfRange);
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
            }
        }
        /// <summary>
        /// 修改学生的成绩
        /// </summary>
        /// <param name="curr">执行此命令的老师</param>
        public static void ChangeScore(this Instructor curr)
        {
            Write("请输入将要修改的学生的账户: ");
            string account = ReadLine();
            if (UserRepository.StudentLibrary == null) return;
            Student temp = UserRepository.StudentLibrary
                           .Find(s => s.Account == account)
                           ?? throw new NullReferenceException();
            Write("分数: ");
            temp[curr.TeachingRange] = double.Parse(ReadLine());
            curr.AddHistory(new Message("你", $"修改了{account}的分数: {temp[curr.TeachingRange]}"));
        }
        /// <summary>
        /// 移除账户   TODO: 需要更优解决方案
        /// </summary>
        public static void RemoveAccount(this HeadTeacher ht)
        {
            Write("请输入将要移除的账户：");
            string account = ReadLine();

            {
                Student student = UserRepository.StudentLibrary.Find(stu => stu.Account == account);
                if (student != null)
                {
                    Write("确定要删除吗？(Y/N) : ");
                    student.ViewPersonalInformation();
                    string result = ReadLine();
                    if (result.Equals("y")
                        || result.Equals("Y")
                        || result.Equals(string.Empty))
                    {
                        UserRepository.StudentLibrary.Remove(student);
                        UserRepository.HeadTeacherUser.AddHistory(new Message("你", $"删除了一个学生账户({account})"));
                        DisplayTheInformationOfSuccessfully();
                    }
                    return;
                }
            }

            {
                Instructor teacher = UserRepository.TeacherLibrary.Find(t => t.Account == account);
                if (teacher != null)
                {
                    Write("确定要删除吗？(Y/N) : ");
                    teacher.ViewPersonalInformation();
                    string result = ReadLine();
                    if (result.Equals("y") || result.Equals("Y") || result.Equals(string.Empty))
                    {
                        UserRepository.TeacherLibrary.Remove(teacher);
                        UserRepository.HeadTeacherUser.AddHistory(new Message("你", $"删除了一个教师账户({account})"));
                        DisplayTheInformationOfSuccessfully();
                    }
                    return;
                }
            }

            DisplayTheInformationOfErrorCode(ErrorCode.CantFindThisAccount, account);
        }
        #endregion

        /// <summary>
        /// 关于
        /// </summary>
        public static void AboutThisApplication()
        {
            Clear();
            ForegroundColor = ConsoleColor.White;
            WriteLine("-------------------------------------------------------------------------");
            WriteLine("  [ Student Manager Studio <SMS> ]");
            WriteLine("  Application Version：V1.0.0");
            WriteLine("  Developer: Sébastien Muller(sebastienenchine@outlook.com)");
            WriteLine("  Please Login to use this program");
            WriteLine("  Enter GetHelp to get help");
            WriteLine("-------------------------------------------------------------------------");
            DefaultColor();
        }
        /// <summary>
        /// 程序默认设置
        /// </summary>
        public static void DefaultSetting()
        {
            DefaultSize();
            DefaultColor();
            DefaultTitle();
        }
        /// <summary>
        /// 程序窗口大小默认设置
        /// </summary>
        public static void DefaultSize()
        {
            SetWindowSize(113, 41);
        }
        /// <summary>
        /// 程序窗口颜色默认设置
        /// </summary>
        public static void DefaultColor()
        {
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.Blue;
        }
        /// <summary>
        /// 程序窗口标题默认设置
        /// </summary>
        public static void DefaultTitle()
        {
            Title = "ClassManager";
        }

        /// <summary>
        /// 获取用户输入的密码
        /// </summary>
        /// <returns>密码</returns>
        public static string EnterPasswd()
        {
            string key = string.Empty;
            ConsoleKeyInfo keyInfo;
            while (true)
            {
                keyInfo = ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Enter) //按下回车，结束
                {
                    break;
                }

                if (keyInfo.Key == ConsoleKey.Backspace && key.Length > 0) //如果是退格键并且字符没有删光
                {
                    --CursorLeft;
                    Write(" ");
                    --CursorLeft;
                    key = key.Substring(0, key.Length - 1);
                }
                else  //过滤掉功能按键等
                {
                    if (char.IsControl(keyInfo.KeyChar))
                    {
                        continue;
                    }

                    key += keyInfo.KeyChar.ToString();
                    Write("*");
                }
            }
            WriteLine();
            return key;
        }
        /// <summary>
        /// 获取登录信息
        /// </summary>
        /// <returns>登录信息</returns>
        public static Tuple<string, string> GetInformationForLogin()
        {
            Write("账号: ");
            string account = ReadLine();
            Write("密码: ");
            string passwd = EnterPasswd();
            return Tuple.Create(account, passwd);
        }

        /// <summary>
        /// 显示错误代码信息
        /// </summary>
        /// <param name="errorCode">错误代码</param>
        /// <param name="parameter">错误参数信息</param>
        public static void DisplayTheInformationOfErrorCode(ErrorCode code, string parameter = "parameter")
        {
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.DarkRed;
            switch (code)
            {
                case ErrorCode.NotACommand:
                    WriteLine($"{parameter} : 无法将 \"{parameter}\" 识别为可执行命令的名称。请检查名称的拼写，然后再试一次。");
                    break;
                case ErrorCode.AccountOrPasswdError:
                    WriteLine("账号或密码错误, 请检查账号和密码的正确性，然后再试一次");
                    break;
                case ErrorCode.AccountAlreadyExists:
                    WriteLine($"{parameter} : 该账户已存在, 请检查此项， 然后再试一次");
                    break;
                case ErrorCode.CantFindThisAccount:
                    WriteLine($"{parameter}: 找不到此账户, 请检查此项, 然后再试一次");
                    break;
                case ErrorCode.ArgumentError:
                    WriteLine("参数错误, 请检查该项，然后再试一次");
                    break;
                case ErrorCode.InconsistentPassword:
                    WriteLine("两次输入密码不一致, 请检查该项，然后再试一次");
                    break;
                case ErrorCode.PasswdError:
                    WriteLine("密码错误, 请检查该项，然后再试一次");
                    break;
                case ErrorCode.NoDisplayableInformation:
                    WriteLine("目前还没有可显示信息");
                    break;
                case ErrorCode.CantAdd:
                    WriteLine("目前已经有两张课表， 请在第一张课表失效后重试");
                    break;
                case ErrorCode.NotSubscribedYet:
                    WriteLine("你没有订阅班主任, 无法取消订阅");
                    break;
                case ErrorCode.DuplicateSubscriptions:
                    WriteLine("你已经订阅了班主任, 无法重复订阅");
                    break;
                case ErrorCode.ArgumentOutOfRange:
                    WriteLine("参数过大或过小");
                    break;
                case ErrorCode.BadAccount:
                    WriteLine("账号不能为空或者包含空格");
                    break;
                case ErrorCode.BadPasswd:
                    WriteLine("密码不能为空或者包含空格");
                    break;
                default:
                    break;
            }
            DefaultColor();
        }
        /// <summary>
        /// 操作成功提示
        /// </summary>
        public static void DisplayTheInformationOfSuccessfully(string msg = "(已完成)")
        {
            //BackgroundColor = ConsoleColor.Black;
            //ForegroundColor = ConsoleColor.Green;
            //Beep();
            //WriteLine(msg);
            //DefaultColor();
            Process.Start(@"D:\Document\Workspace\C#\ConsoleApps\ClassManager\DisplayInfo\bin\Debug\DisplayInfo.exe", msg);
        }
        /// <summary>
        /// 将字符串以指定颜色显示
        /// </summary>
        /// <param name="msg">字符串</param>
        /// <param name="bg">背景颜色</param>
        /// <param name="fg">前景颜色</param>
        public static void PrintColorMsg(string msg, ConsoleColor bg, ConsoleColor fg)
        {
            BackgroundColor = bg;
            ForegroundColor = fg;
            Write(msg);
            DefaultColor();
        }
    }
}