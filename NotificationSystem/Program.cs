using System;
using System.Collections.Generic;

namespace AdminNamespace
{
    public class Admin
    {
        public int Id;
        public string Username;
        public string Email;
        public string Password;
        public List<PostNamespace.Notification> Notifications = new();
        public List<PostNamespace.Post> Posts = new();
    }
}

namespace UserNamespace
{
    public class User
    {
        public int Id;
        public string Name;
        public string Surname;
        public int Age;
        public string Email;
        public string Password;
    }
}

namespace PostNamespace
{
    using UserNamespace;

    public class Post
    {
        public int Id;
        public string Content;
        public DateTime CreationDateTime;
        public int LikeCount = 0;
        public int ViewCount = 0;
    }

    public class Notification
    {
        public int Id;
        public string Text;
        public DateTime DateTime;
        public User FromUser;
    }
}

namespace NetworkNamespace
{
    using AdminNamespace;
    using PostNamespace;

    public static class MailService
    {
        public static void SendMailToAdmin(Admin admin, Notification notification)
        {
            Console.WriteLine($"\n📧 [MAIL] To Admin ({admin.Email}): {notification.Text}\n");
        }
    }
}

class Program
{
    static List<PostNamespace.Post> allPosts = new();
    static AdminNamespace.Admin admin = new AdminNamespace.Admin
    {
        Id = 1,
        Username = "admin",
        Email = "admin@mail.com",
        Password = "1234"
    };

    static List<UserNamespace.User> users = new List<UserNamespace.User>
    {
        new UserNamespace.User { Id = 1, Name = "Violet", Surname = "Rahimova", Age = 21, Email = "violet@mail.com", Password = "pass" }
    };

    static void Main(string[] args)
    {
        // Demo üçün 2 post əlavə edilir
        allPosts.Add(new PostNamespace.Post { Id = 1, Content = "Welcome to Violet's Blog!", CreationDateTime = DateTime.Now });
        allPosts.Add(new PostNamespace.Post { Id = 2, Content = "Second post about C# OOP", CreationDateTime = DateTime.Now });

        Console.Write("Login as (admin/user): ");
        string role = Console.ReadLine()?.ToLower();

        Console.Write("Email or Username: ");
        string login = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        if (role == "admin" && (login == admin.Username || login == admin.Email) && password == admin.Password)
        {
            AdminPanel();
        }
        else if (role == "user")
        {
            var user = users.Find(u => (u.Email == login) && u.Password == password);
            if (user != null)
            {
                UserPanel(user);
            }
            else Console.WriteLine("Invalid user credentials.");
        }
        else
        {
            Console.WriteLine("Login failed.");
        }
    }

    static void UserPanel(UserNamespace.User user)
    {
        while (true)
        {
            Console.WriteLine("\n👤 User Menu:\n1. View Post\n2. Like Post\n0. Exit");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Enter Post ID to view: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    var post = allPosts.Find(p => p.Id == id);
                    if (post != null)
                    {
                        post.ViewCount++;
                        Console.WriteLine($"\n Content: {post.Content}\n Views: {post.ViewCount} |  Likes: {post.LikeCount}");
                        SendNotification(user, $"User {user.Name} viewed post {id}");
                    }
                    else Console.WriteLine(" Post not found.");
                }
            }
            else if (choice == "2")
            {
                Console.Write("Enter Post ID to like: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    var post = allPosts.Find(p => p.Id == id);
                    if (post != null)
                    {
                        post.LikeCount++;
                        Console.WriteLine("You liked the post.");
                        SendNotification(user, $"User {user.Name} liked post {id}");
                    }
                    else Console.WriteLine("Post not found.");
                }
            }
            else break;
        }
    }

    static void AdminPanel()
    {
        Console.WriteLine("\nWelcome Admin!\nAll Notifications:");
        foreach (var notif in admin.Notifications)
        {
            Console.WriteLine($"[{notif.DateTime}] From {notif.FromUser.Name}: {notif.Text}");
        }

        Console.WriteLine("\nCurrent Posts:");
        foreach (var post in allPosts)
        {
            Console.WriteLine($"ID: {post.Id} | {post.Content} |  {post.ViewCount} |  {post.LikeCount}");
        }
    }

    static void SendNotification(UserNamespace.User fromUser, string text)
    {
        var notification = new PostNamespace.Notification
        {
            Id = admin.Notifications.Count + 1,
            Text = text,
            DateTime = DateTime.Now,
            FromUser = fromUser
        };
        admin.Notifications.Add(notification);
        NetworkNamespace.MailService.SendMailToAdmin(admin, notification);
    }
}