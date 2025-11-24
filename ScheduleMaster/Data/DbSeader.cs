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
        public static async Task SeedAsync(ScheduleMasterDbContext context)
        {

            // ============ СОЗДАНИЕ ПОЛЬЗОВАТЕЛЕЙ И СТУДИИ ============

            // Вокальная студия "Вдохновение" - уникальные участники с вымышленными именами
            var vocalUsers = new[]
            {
                new { Surname = "Смирнова", Name = "Виктория", MiddleName = "Валерьевна", Email = "smirnova.v1@mail.ru", Group = "ХТ-22-06", Faculty = "Факультет химической технологии", IsLeader = false },
                new { Surname = "Кузнецова", Name = "Маргарита", MiddleName = "Юрьевна", Email = "kuznetsova.m1@mail.ru", Group = "ХТ-22-07", Faculty = "Факультет химической технологии", IsLeader = false },
                new { Surname = "Волкова", Name = "Вера", MiddleName = "Владиславовна", Email = "volkova.v1@mail.ru", Group = "МСМ-24-06", Faculty = "Факультет машиностроения", IsLeader = false },
                new { Surname = "Сорокина", Name = "Екатерина", MiddleName = "Дмитриевна", Email = "sorokina.e1@mail.ru", Group = "КЗ-21-12", Faculty = "Факультет комплексной безопасности", IsLeader = false },
                new { Surname = "Павлова", Name = "Валерия", MiddleName = "Андреевна", Email = "pavlova.v1@mail.ru", Group = "ТП-25-04", Faculty = "Факультет трубопроводного транспорта", IsLeader = false },
                new { Surname = "Морозова", Name = "Антонина", MiddleName = "Руслановна", Email = "morozova.a1@mail.ru", Group = "ЭЭв-22-02", Faculty = "Факультет электроэнергетики", IsLeader = false },
                new { Surname = "Петров", Name = "Степан", MiddleName = "Дмитриевич", Email = "petrov.s1@mail.ru", Group = "КА-21-07", Faculty = "Факультет автоматизации и вычислительной техники", IsLeader = false },
                new { Surname = "Сергеева", Name = "Екатерина", MiddleName = "Николаевна", Email = "sergeeva.e1@mail.ru", Group = "РИ-25-12", Faculty = "Факультет разведки и инжиниринга", IsLeader = false },
                new { Surname = "Иванов", Name = "Роман", MiddleName = "Сергеевич", Email = "ivanov.r1@mail.ru", Group = "АС-22-05", Faculty = "Факультет автоматики систем", IsLeader = false },
                new { Surname = "Александров", Name = "Давид", MiddleName = "Геннадьевич", Email = "aleksandrov.d1@mail.ru", Group = "КЦ-25-08", Faculty = "Факультет экономики и управления", IsLeader = false },
                new { Surname = "Лебедева", Name = "Виталина", MiddleName = "Вениаминовна", Email = "lebedeva.v1@mail.ru", Group = "КБ-23-19", Faculty = "Факультет биотехнологий", IsLeader = false },
                new { Surname = "Соколова", Name = "Милана", MiddleName = "Павловна", Email = "sokolova.m1@mail.ru", Group = "КН-25-09", Faculty = "Факультет нефтегазовых технологий", IsLeader = false },
                new { Surname = "Андреева", Name = "Мария", MiddleName = "Евгеньевна", Email = "andreeva.m1@mail.ru", Group = "БМ-25-03", Faculty = "Факультет бизнеса и менеджмента", IsLeader = false },
                new { Surname = "Новиков", Name = "Виктор", MiddleName = "", Email = "novikov.v1@mail.ru", Group = "БМ-25-03", Faculty = "Факультет бизнеса и менеджмента", IsLeader = false },
                new { Surname = "Орлов", Name = "Александр", MiddleName = "Викторович", Email = "orlov.a1@mail.ru", Group = "МР-25-02", Faculty = "Факультет механики и робототехники", IsLeader = false },
                new { Surname = "Федорова", Name = "Дарья", MiddleName = "Андреевна", Email = "fedorova.d1@mail.ru", Group = "КН-24-10", Faculty = "Факультет нефтегазовых технологий", IsLeader = false },
                new { Surname = "Романова", Name = "Елена", MiddleName = "", Email = "romanova.e1@mail.ru", Group = "БМ-24-04", Faculty = "Факультет бизнеса и менеджмента", IsLeader = false },
                new { Surname = "Белова", Name = "Наталья", MiddleName = "", Email = "belova.n1@mail.ru", Group = "КН-22-08", Faculty = "Факультет нефтегазовых технологий", IsLeader = false },
                new { Surname = "Козлова", Name = "Анна", MiddleName = "", Email = "kozlova.a1@mail.ru", Group = "ХТ-24-08", Faculty = "Факультет химической технологии", IsLeader = false },
                new { Surname = "Лаврин", Name = "Никита", MiddleName = "", Email = "lavrin.n1@mail.ru", Group = "РБ-24-01", Faculty = "Факультет разведки и инжиниринга", IsLeader = false },
                new { Surname = "Гавриков", Name = "Кирилл", MiddleName = "", Email = "gavrikov.k1@mail.ru", Group = "АС-23-05", Faculty = "Факультет автоматики систем", IsLeader = false },
                new { Surname = "Комаров", Name = "Игорь", MiddleName = "", Email = "komarov.i1@mail.ru", Group = "МТМ-25-03", Faculty = "Факультет машиностроения", IsLeader = false },
                new { Surname = "Соловьев", Name = "Михаил", MiddleName = "", Email = "solovyev.m1@mail.ru", Group = "ТП-25-04", Faculty = "Факультет трубопроводного транспорта", IsLeader = false },
                new { Surname = "Евстигнеев", Name = "Сергей", MiddleName = "", Email = "evstigneev.s1@mail.ru", Group = "КА-22-05", Faculty = "Факультет автоматизации и вычислительной техники", IsLeader = false },
                new { Surname = "Захаров", Name = "Виктор", MiddleName = "", Email = "zakharov.v1@mail.ru", Group = "КА-22-05", Faculty = "Факультет автоматизации и вычислительной техники", IsLeader = false },
                // Руководители (студий)
                new { Surname = "Власова", Name = "Людмила", MiddleName = "", Email = "vlasova.l1@mail.ru", Group = "", Faculty = "", IsLeader = true },
                new { Surname = "Васильев", Name = "Константин", MiddleName = "Максимович", Email = "vasiliev.k1@mail.ru", Group = "", Faculty = "", IsLeader = true }
            };

            // Создаём или выбираем категорию
            var vocalCategory = await context.StudiosCategories.FirstOrDefaultAsync(x => x.Category == "Вокал");
            if (vocalCategory == null)
            {
                vocalCategory = new StudioCategory { Category = "Вокал" };
                context.StudiosCategories.Add(vocalCategory);
                await context.SaveChangesAsync();
            }

            // Создаём студию
            var vocalStudio = await context.Studios
                .FirstOrDefaultAsync(s => s.Title == "Вокальная студия \"Вдохновение\"" && s.StudioCategoryId == vocalCategory.Id);

            if (vocalStudio == null)
            {
                vocalStudio = new Studio
                {
                    Id = Guid.NewGuid(),
                    Title = "Вокальная студия \"Вдохновение\"",
                    StudioCategoryId = vocalCategory.Id
                };
                context.Studios.Add(vocalStudio);
                await context.SaveChangesAsync();
            }

            // Добавляем пользователей
            var userDict = new Dictionary<string, Guid>();
            foreach (var u in vocalUsers)
            {
                var existingUser = await context.Users.FirstOrDefaultAsync(x => x.Email == u.Email);
                Guid userId;
                if (existingUser == null)
                {
                    var newUser = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = u.Email,
                        Surname = u.Surname,
                        Name = u.Name,
                        MiddleName = u.MiddleName,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                        Role = "user",
                        Faculty = u.Faculty,
                        GroupName = u.Group
                    };
                    context.Users.Add(newUser);
                    await context.SaveChangesAsync();
                    userId = newUser.Id;
                }
                else
                {
                    userId = existingUser.Id;
                }
                userDict[u.Email] = userId;
            }

            // Добавляем связь "студия–участник/руководитель" без дубликатов
            foreach (var u in vocalUsers)
            {
                Guid userId = userDict[u.Email];
                var existingStudioUser = await context.StudiosUsers
                    .FirstOrDefaultAsync(x => x.StudentId == userId && x.StudioId == vocalStudio.Id);

                string studioRole = u.IsLeader ? "руководитель" : "участник";
                if (existingStudioUser == null)
                {
                    context.StudiosUsers.Add(new StudioUser
                    {
                        StudentId = userId,
                        StudioId = vocalStudio.Id,
                        StudioRole = studioRole
                    });
                }
                else
                {
                    // Если запись есть, просто обновляем роль на руководитель, если статус руководителя
                    if (u.IsLeader && existingStudioUser.StudioRole != "руководитель")
                        existingStudioUser.StudioRole = "руководитель";
                }
            }
            await context.SaveChangesAsync();

            // ============ СОЗДАНИЕ ГРУПП ============

            // Определяем курс по году выпуска группы
            int GetCourseByGroupCode(string groupCode)
            {
                if (string.IsNullOrEmpty(groupCode) || groupCode.Length < 3)
                    return 0;

                // Извлекаем год (две цифры после букв факультета)
                var yearMatch = System.Text.RegularExpressions.Regex.Match(groupCode, @"(\d{2})");
                if (!int.TryParse(yearMatch.Groups[1].Value, out int year))
                    return 0;

                // Текущий год 2025, вычисляем курс
                int currentYear = 2025;
                int courseYear = 2000 + year;
                int course = currentYear - courseYear + 1;

                return course > 0 && course <= 6 ? course : 0;
            }

            // Разделяем участников на группы
            var maleMembers = new List<(string Email, Guid UserId)>(); // мужчины
            var femaleMembers = new List<(string Email, Guid UserId)>(); // женщины
            var seniorMembers = new List<(string Email, Guid UserId)>(); // рабочая группа (3-5 курсы)
            var juniorMembers = new List<(string Email, Guid UserId)>(); // младшая группа (1-2 курс)

            // Типичные мужские имена для определения пола (вымышленные)
            var maleNames = new[] { "Роман", "Давид", "Степан", "Константин", "Игорь", "Сергей", "Виктор", "Кирилл", "Михаил", "Никита", "Александр" };

            foreach (var u in vocalUsers)
            {
                if (!userDict.ContainsKey(u.Email))
                    continue;

                Guid userId = userDict[u.Email];
                bool isMale = maleNames.Contains(u.Name);

                if (isMale)
                    maleMembers.Add((u.Email, userId));
                else
                    femaleMembers.Add((u.Email, userId));

                // Определяем курс и добавляем в соответствующую группу
                int course = GetCourseByGroupCode(u.Group);
                if (course >= 3 && course <= 5)
                    seniorMembers.Add((u.Email, userId));
                else if (course >= 1 && course <= 2)
                    juniorMembers.Add((u.Email, userId));
            }

            // Создаём группы
            async Task<Group> EnsureGroupAsync(string title, Guid studioId)
            {
                var group = await context.Groups.FirstOrDefaultAsync(g => g.StudioId == studioId && g.Title == title);
                if (group == null)
                {
                    group = new Group { Id = Guid.NewGuid(), StudioId = studioId, Title = title };
                    context.Groups.Add(group);
                    await context.SaveChangesAsync();
                }
                return group;
            }

            var groupMale = await EnsureGroupAsync("Мужская группа", vocalStudio.Id);
            var groupFemale = await EnsureGroupAsync("Женская группа", vocalStudio.Id);
            var groupSenior = await EnsureGroupAsync("Рабочая группа (3-5 курсы)", vocalStudio.Id);
            var groupJunior = await EnsureGroupAsync("Младшая группа (1-2 курс)", vocalStudio.Id);

            // Добавляем участников в группы
            foreach (var (email, userId) in maleMembers)
            {
                var existingGroupUser = await context.GroupsUsers
                    .FirstOrDefaultAsync(x => x.StudentId == userId && x.GroupId == groupMale.Id);
                if (existingGroupUser == null)
                {
                    context.GroupsUsers.Add(new GroupUser { StudentId = userId, GroupId = groupMale.Id });
                }
            }

            foreach (var (email, userId) in femaleMembers)
            {
                var existingGroupUser = await context.GroupsUsers
                    .FirstOrDefaultAsync(x => x.StudentId == userId && x.GroupId == groupFemale.Id);
                if (existingGroupUser == null)
                {
                    context.GroupsUsers.Add(new GroupUser { StudentId = userId, GroupId = groupFemale.Id });
                }
            }

            foreach (var (email, userId) in seniorMembers)
            {
                var existingGroupUser = await context.GroupsUsers
                    .FirstOrDefaultAsync(x => x.StudentId == userId && x.GroupId == groupSenior.Id);
                if (existingGroupUser == null)
                {
                    context.GroupsUsers.Add(new GroupUser { StudentId = userId, GroupId = groupSenior.Id });
                }
            }

            foreach (var (email, userId) in juniorMembers)
            {
                var existingGroupUser = await context.GroupsUsers
                    .FirstOrDefaultAsync(x => x.StudentId == userId && x.GroupId == groupJunior.Id);
                if (existingGroupUser == null)
                {
                    context.GroupsUsers.Add(new GroupUser { StudentId = userId, GroupId = groupJunior.Id });
                }
            }

            await context.SaveChangesAsync();

            // ============ СОЗДАНИЕ СОБЫТИЙ ============
            var now = DateTime.UtcNow;
            var baseDate = now.AddDays(1); // Начинаем с завтрашнего дня

            // 1) Концерт для всей студии (все группы)
            var concertEvent = await context.Events
                .FirstOrDefaultAsync(e => e.Title == "Концерт вокальной студии \"Вдохновение\"");
            if (concertEvent == null)
            {
                concertEvent = new Event
                {
                    Id = Guid.NewGuid(),
                    Title = "Концерт вокальной студии \"Вдохновение\"",
                    StartDateTime = baseDate.AddDays(7).AddHours(18),
                    EndDateTime = baseDate.AddDays(7).AddHours(20),
                    Location = "Концертный зал"
                };
                context.Events.Add(concertEvent);

                // Добавляем концерт для всех групп студии
                context.EventsStudios.Add(new EventStudio { EventId = concertEvent.Id, StudioId = vocalStudio.Id });
                context.EventsGroups.Add(new EventGroup { EventId = concertEvent.Id, GroupId = groupMale.Id });
                context.EventsGroups.Add(new EventGroup { EventId = concertEvent.Id, GroupId = groupFemale.Id });
                context.EventsGroups.Add(new EventGroup { EventId = concertEvent.Id, GroupId = groupSenior.Id });
                context.EventsGroups.Add(new EventGroup { EventId = concertEvent.Id, GroupId = groupJunior.Id });

                await context.SaveChangesAsync();
            }


            // 2) Занятие для мужской группы
            var rehearsalEvent = await context.Events
                .FirstOrDefaultAsync(e => e.Title == "Репетиция - Мужская группа");
            if (rehearsalEvent == null)
            {
                rehearsalEvent = new Event
                {
                    Id = Guid.NewGuid(),
                    Title = "Репетиция - Мужская группа",
                    StartDateTime = baseDate.AddHours(15),
                    EndDateTime = baseDate.AddHours(16).AddMinutes(30),
                    Location = "Студия \"Вдохновения\""
                };

                context.Events.Add(rehearsalEvent);
                context.EventsGroups.Add(new EventGroup { EventId = rehearsalEvent.Id, GroupId = groupMale.Id });
                context.EventsStudios.Add(new EventStudio { EventId = rehearsalEvent.Id, StudioId = vocalStudio.Id });

                await context.SaveChangesAsync();
            }

            // 3) Собрание для всех групп
            var meetingEvent = await context.Events
                .FirstOrDefaultAsync(e => e.Title == "Собрание студии \"Вдохновение\"");
            if (meetingEvent == null)
            {
                meetingEvent = new Event
                {
                    Id = Guid.NewGuid(),
                    Title = "Собрание студии \"Вдохновение\"",
                    StartDateTime = baseDate.AddDays(14).AddHours(17),
                    EndDateTime = baseDate.AddDays(14).AddHours(18),
                    Location = "Студия \"Вдохновения\""
                };
                context.Events.Add(meetingEvent);

                context.EventsStudios.Add(new EventStudio { EventId = meetingEvent.Id, StudioId = vocalStudio.Id });
                context.EventsGroups.Add(new EventGroup { EventId = meetingEvent.Id, GroupId = groupMale.Id });
                context.EventsGroups.Add(new EventGroup { EventId = meetingEvent.Id, GroupId = groupFemale.Id });
                context.EventsGroups.Add(new EventGroup { EventId = meetingEvent.Id, GroupId = groupSenior.Id });
                context.EventsGroups.Add(new EventGroup { EventId = meetingEvent.Id, GroupId = groupJunior.Id });
                await context.SaveChangesAsync();
            }

            // 4) Совместные занятия для Рабочей и Мужской группы
            var jointRehearsalEvent = await context.Events
                .FirstOrDefaultAsync(e => e.Title == "Совместная репетиция - Рабочая и Мужская группы");
            if (jointRehearsalEvent == null)
            {
                jointRehearsalEvent = new Event
                {
                    Id = Guid.NewGuid(),
                    Title = "Совместная репетиция - Рабочая и Мужская группы",
                    StartDateTime = baseDate.AddDays(20).AddHours(17),
                    EndDateTime = baseDate.AddDays(20).AddHours(18).AddMinutes(30),
                    Location = "Студия \"Вдохновения\""
                };
                context.Events.Add(jointRehearsalEvent);
                context.EventsStudios.Add(new EventStudio { EventId = jointRehearsalEvent.Id, StudioId = vocalStudio.Id });
                context.EventsGroups.Add(new EventGroup { EventId = jointRehearsalEvent.Id, GroupId = groupSenior.Id });
                context.EventsGroups.Add(new EventGroup { EventId = jointRehearsalEvent.Id, GroupId = groupMale.Id });
                await context.SaveChangesAsync();
            }

            await context.SaveChangesAsync();

        }
    }
}
