using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleMaster.Data;
using ScheduleMaster.DTOs;
using ScheduleMaster.Models;
using ScheduleMaster.Services;

namespace ScheduleMaster.DbSeader
{
    public class DbSeeder
    {
        public static async Task SeedAsync(
            ScheduleMasterDbContext context,
            UserService userService,
            StudioService studioService,
            GroupService groupService,
            ScheduleService scheduleService)
        {
            // Очистка таблиц (если нужно)
            context.Events.RemoveRange(context.Events);
            context.GroupsUsers.RemoveRange(context.GroupsUsers);
            context.StudiosUsers.RemoveRange(context.StudiosUsers);
            context.Groups.RemoveRange(context.Groups);
            context.Studios.RemoveRange(context.Studios);
            context.Users.RemoveRange(context.Users);
            await context.SaveChangesAsync();

            // Создаем админа
            // var admin = await userService.CreateUserAsync(new CreateUserDTO
            // {
            //     Email = "admin",
            //     Password = "admin",
            //     Surname = "Ильичев",
            //     Name = "Роман",
            //     Role = "admin"
            // });

            // // Создаем руководителей
            // var leader1 = await userService.CreateUserAsync(new CreateUserDTO
            // {
            //     Email = "leader1@mail.com",
            //     Password = "user",
            //     Surname = "Иванов",
            //     Name = "Иван",
            //     Role = "user"
            // });

            // var leader2 = await userService.CreateUserAsync(new CreateUserDTO
            // {
            //     Email = "leader2@mail.com",
            //     Password = "user",
            //     Surname = "Петров",
            //     Name = "Петр",
            //     Role = "user"
            // });

            // // Создаем пользователей
            // var userIds = new List<Guid>();
            // for (int i = 1; i <= 10; i++)
            // {
            //     var u = await userService.CreateUserAsync(new CreateUserDTO
            //     {
            //         Email = $"user{i}@mail.com",
            //         Password = "user",
            //         Surname = $"Фамилия{i}",
            //         Name = $"Имя{i}",
            //         Role = "user"
            //     });
            //     userIds.Add(u.Id);
            // }

            // // Создаем студии
            // var studio1 = await studioService.CreateStudioAsync(new CreateStudioDTO
            // {
            //     Name = "Вокальная студия",
            //     Category = "Вокал",
            //     AdministratorId = leader1.Id
            // });

            // var studio2 = await studioService.CreateStudioAsync(new CreateStudioDTO
            // {
            //     Name = "Танцевальная студия",
            //     Category = "Танцы",
            //     AdministratorId = leader2.Id
            // });

            // // Создаем группы
            // var groups1 = new List<Guid>();
            // for (int i = 1; i <= 3; i++)
            // {
            //     var group = await groupService.CreateGroupAsync(new CreateGroupDTO
            //     {
            //         StudioId = studio1.Id,
            //         Name = $"Группа {i} студии 1"
            //     });
            //     groups1.Add(group.Id);
            // }

            // var groups2 = new List<Guid>();
            // for (int i = 1; i <= 3; i++)
            // {
            //     var group = await groupService.CreateGroupAsync(new CreateGroupDTO
            //     {
            //         StudioId = studio2.Id,
            //         Name = $"Группа {i} студии 2"
            //     });
            //     groups2.Add(group.Id);
            // }

            // // Добавляем пользователей в студии (StudioMemberships)
            // for (int i = 0; i < userIds.Count; i++)
            // {
            //     var studioId = i < 5 ? studio1.Id : studio2.Id;
            //     context.StudioMemberships.Add(new StudioMembership
            //     {
            //         StudentId = userIds[i],
            //         StudioId = studioId
            //     });
            // }

            // // Добавляем пользователей в группы (GroupMemberships)
            // // Первые 5 пользователей в группы студии 1
            // for (int i = 0; i < 5; i++)
            // {
            //     context.GroupMemberships.Add(new GroupMembership
            //     {
            //         StudentId = userIds[i],
            //         GroupId = groups1[i % groups1.Count]
            //     });
            // }
            // // Остальные 5 пользователей в группы студии 2
            // for (int i = 5; i < 10; i++)
            // {
            //     context.GroupMemberships.Add(new GroupMembership
            //     {
            //         StudentId = userIds[i],
            //         GroupId = groups2[i % groups2.Count]
            //     });
            // }

            // await context.SaveChangesAsync();

            // // Создаем расписания для студии 1
            // for (int i = 1; i <= 5; i++)
            // {
            //     await scheduleService.CreateScheduleAsync(new CreateScheduleDTO
            //     {
            //         StudioId = studio1.Id,
            //         GroupId = groups1[i % groups1.Count],
            //         StartDateTime = DateTime.UtcNow.AddDays(i),
            //         EndDateTime = DateTime.UtcNow.AddDays(i).AddHours(1),
            //         Location = $"Кабинет {i}",
            //         WeekType = (i % 2 == 0) ? "верхняя" : "нижняя",
            //         IsRecurring = true
            //     });
            // }

            // // Создаем расписания для студии 2
            // for (int i = 1; i <= 4; i++)
            // {
            //     await scheduleService.CreateScheduleAsync(new CreateScheduleDTO
            //     {
            //         StudioId = studio2.Id,
            //         GroupId = groups2[i % groups2.Count],
            //         StartDateTime = DateTime.UtcNow.AddDays(i),
            //         EndDateTime = DateTime.UtcNow.AddDays(i).AddHours(1),
            //         Location = $"Зал {i}",
            //         WeekType = (i % 2 == 0) ? "верхняя" : "нижняя",
            //         IsRecurring = false
            //     });
            // }
        }
    }
}
