﻿using System;
using static System.Console;
using Sebastien.ClassManager.Enums;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Xml;
using System.Text;
using System.IO;

#region 其他需求
//其他: 
//          2: 切换用户之后 窗口标题显示对应用户类型
#endregion

#region 扩展待实现
//学生：
//          3：查看班主任信息
//教师：
//          3：查看班主任信息
//班主任
//          1：
#endregion

#region 待解决问题
//中英文对齐问题   
#endregion

namespace Sebastien.ClassManager.Core
{
    /// <summary>
    /// 客户端
    /// </summary>
    public sealed class Client
    {
        public static string FileName { get; } = @"D:\Document\TEXT\班级管理系统留言板";
        /// <summary>
        /// 程序状态
        /// </summary>
        private static State _appState = State.On;
        /// <summary>
        /// 检查是否已经启动了此程序
        /// </summary>
        /// <returns>如果已经启动了此程序则为<see>true</see>, 否则为<see>false</see></returns>
        private static bool IsStarted()
        {
            var mutex = new Mutex(false, "ClassManagerLock", out bool Started);
            return Started;
        }
        /// <summary>
        /// 程序主逻辑
        /// </summary>
        /// <param name="args">命令行参数</param>
        private static void Main(string[] args)
        {
            if (!IsStarted())
            {
                WriteLine("此程序已在运行中, 按任意键关闭此窗口");
                ReadKey(true);
                return;
            }
            Clear();
            Ui.AddStudents();
            Ui.DefaultSetting();
            Ui.AboutThisApplication();

            UserCore currentUser;
            do
            {
                currentUser = UserCore.Login();
            } while (currentUser == null);

            while (_appState == State.On)
            {
                currentUser.Prompt();
                switch (currentUser.UserType)
                {
                    case Identity.Student:
                        currentUser = RunForStudent(currentUser as Student) ?? currentUser;
                        break;
                    case Identity.Instructor:
                        currentUser = RunForTeacher(currentUser as Instructor) ?? currentUser;
                        break;
                    case Identity.HeadTeacher:
                        currentUser = RunForHeadTeacher(currentUser as HeadTeacher) ?? currentUser;
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
        }

        /// <summary>
        /// 加载选择器
        /// </summary>
        /// <returns>选择器对象</returns>
        public static object GetSelectorObject<T>(List<string> info, params T[] selects) //TODO:
        {
            var asm = Assembly.LoadFrom(@"D:\Document\Workspace\C_SHARP\ConsoleApps\ClassManager\SelectorLib\bin\Debug\SelectorLib.dll");
            Type coreTypeName = asm.GetType("MySelector.Selector`1"); //1为泛型类型个数, 如Test<T>类, 因此 如果是2, 则为: Test<T1, T2> 
            Type fullTypeName = coreTypeName.MakeGenericType(typeof(T));
            return asm.CreateInstance(fullTypeName.FullName ?? throw new InvalidOperationException(), true, BindingFlags.Default, null, new object[] { info, selects }, null, null);
        }
        /// <summary>
        /// 获取由用户输入的命令
        /// </summary>
        /// <param name="input" />
        /// <param name="cmd">命令参数</param>
        /// <returns></returns>
        public static string GetCmd(string input, out Command cmd)
        {
            if (Enum.TryParse(input, true, out cmd))
            {
                return input;
            }

            Ui.DisplayTheInformationOfErrorCode(ErrorCode.NotACommand, input);
            return null;
        }
        /// <summary>
        /// 交互(针对于所有用户)
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <param name="cmd">可执行命令</param>
        /// <returns></returns>
        private static UserCore RunForUser(UserCore currentUser, Command cmd)
        {
            UserCore user = null;
            switch (cmd)
            {
                case Command.GetHelp:
                    currentUser.GetHelp();
                    break;
                case Command.SwitchUser:
                    user = currentUser.SwitchUser();
                    break;
                case Command.ShowMe:
                    currentUser.ViewPersonalInformation();
                    break;
                case Command.ChangePasswd:
                    currentUser.CallChangeMyPasswd();
                    break;
                case Command.ChangeAge:
                    currentUser.CallChangeMyAge();
                    break;
                case Command.ChangeAddress:
                    currentUser.ChangeMyAddress();
                    break;
                case Command.ChangeSex:
                    currentUser.ChangeMySex();
                    break;
                case Command.ViewMyHistory:
                    currentUser.ViewMyHistory();
                    break;
                case Command.ViewCurriculums:
                    currentUser.ViewCurriculum();
                    break;
                case Command.ViewHeadTeacher:
                    currentUser.ViewTheInformationOfTheHeadteacher();
                    break;
                case Command.LeaveAMessage:
                    currentUser.LeaveAMessage();
                    break; //Todo:
                case Command.ViewLeaveMessages:
                    currentUser.ViewTheLeaveMessages();
                    break; //Todo:
                case Command.Exit:
                    currentUser.LogOut();
                    _appState = State.Off;
                    break;
                case Command.UrlTest:
                    currentUser.UrlTest();
                    break;
                default:
                    Ui.DisplayTheInformationOfErrorCode(ErrorCode.NotACommand, cmd.ToString());
                    break;
            }
            return user;
        }
        /// <summary>
        /// 交互(针对于学生用户)
        /// </summary>
        /// <param name="stu">学生用户</param>
        /// <returns>用户对象</returns>
        private static UserCore RunForStudent(Student stu)
        {
            stu.TheTipsOfNews();
            ForegroundColor = ConsoleColor.Yellow;
            string input = ReadLine();
            ForegroundColor = ConsoleColor.Blue;
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (GetCmd(input, out Command cmd) == null)
            {
                return null;
            }
            UserCore result = null;
            switch (cmd)
            {
                case Command.Exit:
                case Command.SwitchUser:
                case Command.GetHelp:
                case Command.ShowMe:
                case Command.ChangePasswd:
                case Command.ChangeAge:
                case Command.ChangeAddress:
                case Command.ChangeSex:
                case Command.ViewMyHistory:
                case Command.ViewCurriculums:
                case Command.ViewHeadTeacher:
                case Command.LeaveAMessage:
                case Command.ViewLeaveMessages:
                case Command.UrlTest:
                    result = RunForUser(stu, cmd);
                    break;
                case Command.ViewNews:
                    stu.ViewNews();
                    break;
                case Command.ViewAllNews:
                    stu.ViewTotalNews();
                    break;
                case Command.MyScore:
                    stu.ShowMyScore();
                    break;
                case Command.StudentsPreview:
                    Ui.DisplayStudentList();
                    break;
                case Command.TeachersPreview:
                    stu.DisplayTeacherList();
                    break;
                case Command.SubscriptionToHeadTeacher:
                    stu.SubscriptionToHeadTeacher(UserRepository.HeadTeacherUser);
                    break;
                case Command.UnsubscribeToHeadTeacher:
                    stu.UnsubscribeToHeadTeacher(UserRepository.HeadTeacherUser);
                    break;
                default:
                    Ui.DisplayTheInformationOfErrorCode(ErrorCode.NotACommand, input);
                    break;
            }
            return result;
        }
        /// <summary>
        /// 交互(针对于各科目任课老师用户)
        /// </summary>
        /// <param name="headTeacher">班主任用户</param>
        /// <returns>用户对象</returns>
        private static UserCore RunForTeacher(Instructor teacher)
        {
            ForegroundColor = ConsoleColor.Yellow;
            string input = ReadLine();
            ForegroundColor = ConsoleColor.Blue;
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (GetCmd(input, out Command cmd) == null)
            {
                return null;
            }
            UserCore result = null;
            switch (cmd)
            {
                case Command.Exit:
                case Command.SwitchUser:
                case Command.GetHelp:
                case Command.ShowMe:
                case Command.ChangePasswd:
                case Command.ChangeAge:
                case Command.ChangeAddress:
                case Command.ChangeSex:
                case Command.ViewMyHistory:
                case Command.ViewCurriculums:
                case Command.ViewHeadTeacher:
                case Command.LeaveAMessage:
                case Command.ViewLeaveMessages:
                case Command.UrlTest:
                    result = RunForUser(teacher, cmd);
                    break;
                case Command.StudentsPreview:
                    Ui.DisplayStudentList();
                    break;
                case Command.TeachersPreview:
                    teacher.DisplayTeacherList();
                    break;
                case Command.AllScore:
                    teacher.DisplayAllScoreOfStudent();
                    break;
                case Command.AllScoreAndRank:
                    teacher.DisplayAllScoreOfStudent(State.On, State.On);
                    break;
                case Command.ChangeScore:
                    teacher.CallChangeScore();
                    break;
                case Command.HighThan:
                    teacher.DisplayScoreHighThan();
                    break;
                case Command.ReleaseAMsg:
                    teacher.ReleaseNewMessage();
                    break;
                default:
                    Ui.DisplayTheInformationOfErrorCode(ErrorCode.NotACommand, input);
                    break;
            }
            return result;
        }
        /// <summary>
        /// 交互(班主任用户)
        /// </summary>
        /// <param name="headTeacher"></param>
        /// <returns>用户对象</returns>
        private static UserCore RunForHeadTeacher(HeadTeacher headTeacher)
        {
            ForegroundColor = ConsoleColor.Yellow;
            string input = ReadLine();
            ForegroundColor = ConsoleColor.Blue;
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (GetCmd(input, out Command cmd) == null)
            {
                return null;
            }
            UserCore result = null;
            switch (cmd)
            {
                case Command.Exit:
                case Command.SwitchUser:
                case Command.GetHelp:
                case Command.ShowMe:
                case Command.ChangePasswd:
                case Command.ChangeAge:
                case Command.ChangeAddress:
                case Command.ChangeSex:
                case Command.ViewMyHistory:
                case Command.ViewCurriculums:
                case Command.LeaveAMessage:
                case Command.ViewLeaveMessages:
                case Command.UrlTest:
                    //case Command.ViewHeadTeacher:
                    result = RunForUser(headTeacher, cmd);
                    break;
                case Command.ChangeName:
                    headTeacher.ChangeMyName();
                    break;
                case Command.ChangeNameOfThisUser:
                    headTeacher.ChangeNameOfOtherUser();
                    break;
                case Command.AddStudent:
                    headTeacher.CallAddStudent();
                    break;
                case Command.AddTeacher:
                    headTeacher.CallAddTeacher();
                    break;
                case Command.Remove:
                    headTeacher.RemoveAccount();
                    break;
                case Command.StudentsPreview:
                    Ui.DisplayStudentList();
                    break;
                case Command.TeachersPreview:
                    headTeacher.DisplayTeacherList();
                    break;
                case Command.AllScore:
                    headTeacher.DisplayAllScoreOfStudent();
                    break;
                case Command.AllScoreAndRank:
                    headTeacher.DisplayAllScoreOfStudent(State.On, State.On);
                    break;
                case Command.HighThan:
                    headTeacher.DisplayScoreHighThan();
                    break;
                case Command.ReleaseNewCurriculum:
                    headTeacher.AddNewCurriculum();
                    break;
                case Command.ReleaseAMsg:
                    headTeacher.ReleaseNewMessage();
                    break;
                default:
                    Ui.DisplayTheInformationOfErrorCode(ErrorCode.NotACommand, input);
                    break;
            }



            return result;
        }
        /// <summary>
        /// 检查账户是否存在(旧版本)
        /// </summary>
        /// <param name="account">账户</param>
        /// <returns>true: 不存在， false: 已存在</returns>
        [Obsolete("方法已过期, 此方法依赖User.cs文件中的FindAccount <T> 类, 推荐使用基于本地函数实现的新版本")]
        public static UserCore CheckAccountAvailabilityOldVersionAndNeedOtherClass(string account) //依赖于User.cs文件中的FindAccount <T> 类
        {
            int index1 = UserRepository.StudentLibrary.FindIndex(new FindAccount<Student>(account).FindAccountPredicate);
            if (index1 != -1)
            {
                return UserRepository.StudentLibrary[index1];
            }
            int index2 = UserRepository.TeacherLibrary.FindIndex(new FindAccount<Teacher>(account).FindAccountPredicate);
            if (index2 != -1)
            {
                return UserRepository.StudentLibrary[index2];
            }
            return account == UserRepository.HeadTeacherUser.Account ? UserRepository.HeadTeacherUser : null;
        }
        /// <summary>
        /// 检查账户是否存在(旧版本)
        /// </summary>
        /// <param name="account">账户</param>
        /// <returns>true: 不存在， false: 已存在</returns>
        [Obsolete("方法已过期, 此方法使用Lambda表达式, 但同一表达式使用多次")]
        public static UserCore CheckAccountAvailabilityOldVersionLambda(string account)
        {
            int index1 = UserRepository.StudentLibrary.FindIndex(u => u.Account == account);
            if (index1 != -1)
            {
                return UserRepository.StudentLibrary[index1];
            }
            int index2 = UserRepository.TeacherLibrary.FindIndex(u => u.Account.Equals(account));
            if (index2 != -1)
            {
                return UserRepository.StudentLibrary[index2];
            }
            return account == UserRepository.HeadTeacherUser.Account ? UserRepository.HeadTeacherUser : null;
        }
        /// <summary>
        /// 检查账户是否存在(旧版本)
        /// </summary>
        /// <param name="account">账户</param>
        /// <returns>true: 不存在， false: 已存在</returns>
        [Obsolete("方法已过期, 使用传统for循环的旧版本")]
        public static UserCore CheckAccountAvailabilityOldVersionNormal(string account)
        {
            foreach (Student index in UserRepository.StudentLibrary)
            {
                if (account.Equals(index.Account))
                {
                    return index;
                }
            }
            foreach (Instructor index in UserRepository.TeacherLibrary)
            {
                if (account.Equals(index.Account))
                {
                    return index;
                }
            }
            return account == UserRepository.HeadTeacherUser.Account ? UserRepository.HeadTeacherUser : null;
        }
        /// <summary>
        /// 检查账户是否存在
        /// </summary>
        /// <param name="account">账户</param>
        /// <returns>true: 不存在， false: 已存在</returns>
        public static UserCore CheckAccountAvailability(string account)
        {
            Student student = UserRepository.StudentLibrary.Find(IsEquals);
            if (student != default(Student))
            {
                return student;
            }
            Instructor instructor = UserRepository.TeacherLibrary.Find(IsEquals);
            if (instructor != default(Instructor))
            {
                return instructor;
            }

            return (IsEquals(UserRepository.HeadTeacherUser)) ? UserRepository.HeadTeacherUser : null;

            //Local Function For The Find() Method 
            bool IsEquals(UserCore u) => u.Account.Equals(account);
        }
        /// <summary>
        /// 检查登录信息
        /// </summary>
        /// <param name="loginInformation">登录信息</param>
        /// <returns>用户对象</returns>
        public static UserCore IdentityCheck(Tuple<string, string> loginInformation)
        {
            UserCore result = CheckAccountAvailability(loginInformation.Item1);
            if (result == null)
            {
                return null;
            }

            if (loginInformation.Item2.Equals(result.Passwd))
            {
                return result;
            }
            result.AddHistory(new Message("登录操作", "登录失败"));
            return null;
        }
        /// <summary>
        /// 检查能否发布新课表
        /// </summary>
        /// <returns></returns>
        public static bool CanAddNewCurriculum()
        {
            UpdateCurriculum();
            return UserRepository._curriculums[1] == null;
        }
        /// <summary>
        /// 检查已失效的课表并删除(没有使用" ?. "条件运算符的旧方法)
        /// </summary>
        [Obsolete("方法已过期, 此方法没有使用 ?. 运算符,  代码过于冗长, 请使用此方法的新版本")]
        public static void UpdateCurriculumOldVersion()
        {
            if (UserRepository._curriculums[0] != null)
            {
                if (UserRepository._curriculums[1] != null)
                {
                    if (DateTime.Now > UserRepository._curriculums[0].OverTime)
                    {
                        if (DateTime.Now > UserRepository._curriculums[1].OverTime)
                        {
                            UserRepository._curriculums[0] = UserRepository._curriculums[1] = null;
                        }
                        UserRepository._curriculums[0] = UserRepository._curriculums[1];
                        UserRepository._curriculums[1] = null;
                    }
                }
                else
                {
                    if (DateTime.Now <= UserRepository._curriculums[0].OverTime) return;
                    UserRepository._curriculums[0] = null;
                }
            }
        }
        /// <summary>
        /// 检查已失效的课表并删除(使用" ?. "条件运算符的新方法)
        /// </summary>
        public static void UpdateCurriculum()
        {
            //<!>DateTime结构体变量于null比较始终为false</!>
            if (DateTime.Now > UserRepository._curriculums[0]?.OverTime)
            {
                if (DateTime.Now > UserRepository._curriculums[1]?.OverTime)
                {
                    UserRepository._curriculums[0] = UserRepository._curriculums[1] = null;
                }
                else
                {
                    UserRepository._curriculums[0] = UserRepository._curriculums[1];
                    UserRepository._curriculums[1] = null;
                }
            }
        }
    }
}
