using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.CampusLife;
using Rise.Domain.Dashboard;
using Rise.Domain.Departments;
using Rise.Domain.Education;
using Rise.Domain.Entities;
using Rise.Domain.Events;
using Rise.Domain.Infrastructure;
using Rise.Domain.News;
using Rise.Domain.Notifications;
using Rise.Domain.Users;
using Rise.Shared.CampusLife;
using Rise.Shared.News;
using Rise.Shared.Shortcuts;
using Rise.Shared.Notifications;
using Serilog;

namespace Rise.Persistence;

/// <summary>
/// Seeds the database
/// </summary>
/// <param name="dbContext"></param>
/// <param name="roleManager"></param>
/// <param name="userManager"></param>
public class DbSeeder(
    ApplicationDbContext dbContext,
    RoleManager<IdentityRole> roleManager,
    UserManager<IdentityUser> userManager)
{
    private const string PasswordDefault = "A1b2C3!";

    public async Task SeedAsync()
    {
        Log.Information("Seeding Database");

        await ShortcutsAsync();
        await NotificationsAsync();
        await RolesAsync();
        await DepartmentsAsync();
        await ClassGroupsAsync();
        await UsersAsync();
        await AssignDepartmentsAsync();

        await InfrastructureAsync();
        await MenuAsync();
        await EventsAsync();
        await NewsAsync();
        await LessonsAsync();
        await DeadlinesAsync();

        await StudentClubsAsync();
        await StudentDealsAsync();
        await JobsAsync();


        await dbContext.SaveChangesAsync();
    }

    private async Task NotificationsAsync()
    {
        if (dbContext.Notifications.Any()) return;

        var notificationsList = new[]
        {
            new Notification(
                "Welkom bij RISE",
                @"
                        <p>Dear students</p>

                        <p>
                          Before we can get started, <strong style=""color:black;"">everyone must complete the following form by Monday, September 22, 23:59</strong>:
                        </p>

                        <p>
                          <a style=""color:var(--secondary-color); text-decoration:underline;"" href=""https://forms.office.com/e/PSRYU7CyBc"" target=""_blank"" rel=""noopener"">
                            https://forms.office.com/e/PSRYU7CyBc
                          </a>
                        </p>

                        <p>
                          Welcome to RISE (Real-life Integrated Software Engineering). How this course is organized will be made clear at our kickoff.
                          <strong style=""color:black;"">Attendance is mandatory.</strong>
                        </p>

                        <section>
                          <h2>Kick-off details</h2>

                          <h3>For students from Ghent (in person), IC students (in person), and VC (via stream):</h3>
                          <ul>
                            <li>Wednesday, September 24, 9:00, C.2.010</li>
                            <li>
                              <a style=""color:var(--secondary-color); text-decoration:underline;"" href=""https://teams.microsoft.com/dl/launcher/launcher.html?url=%2F_%23%2Fl%2Fmeetup-join%2F19%3AWvzPgNPXPX5YWjNEAmmUPKY2Y1kADoylLz45xG8Hd_A1%40thread.tacv2%2F1758526997258%3Fcontext%3D%257b%2522Tid%2522%253a%25225cf7310e-091a-4bc5-acd7-26c721d4cccd%2522%252c%2522Oid%2522%253a%25225df9aefe-ecaf-4b24-8629-0ac7c2652d14%2522%257d%26anon%3Dtrue&type=meetup-join&deeplinkId=e2dbf4e6-752b-41dc-b152-e041fca85c9c&directDl=true&msLaunch=true&enableMobilePage=true&suppressPrompt=true"">
                                Live stream: RISE - Kick-off Live Stream | Vergadering-Deelnemen | Microsoft Teams
                              </a>
                            </li>
                          </ul>

                          <h3>For students from Aalst (in person):</h3>
                          <ul>
                            <li>Wednesday, September 24, 11:00, GAARB.0.029</li>
                          </ul>

                          <h3>For TIAO students</h3>
                          <ul>
                            <li>The kick-off will be recorded and made available</li>
                            <li>A specific meeting will be communicated next week</li>
                          </ul>
                        </section>

                        <p>Happy coding!</p>
                            ",
                "https://chamilo.hogent.be/index.php?go=CourseViewer&application=Chamilo%5CApplication%5CWeblcms&course=69909&tool=Announcement&browser=List&tool_action=Viewer&publication=2539030",
                NotificationTopics.Default.Name
            ),
            new Notification(
                "Herinnering: deadlines",
                "Vergeet niet je deadlines te controleren voor je ingeschreven cursussen. Raadpleeg de deadlines pagina voor details.",
                "https://www.google.com/",
                NotificationTopics.Deadline.Name),
            new Notification(
                "IT-onderhoud gepland",
                "Op zaterdag tussen 02:00 en 06:00 uur is er gepland IT-onderhoud. Verwacht korte onderbrekingen van diensten zoals e-mail en het netwerk.",
                "https://www.google.com/",
                NotificationTopics.Default.Name),
            new Notification(
                "Examenuitslagen beschikbaar",
                "De uitslagen van de recente examenzittingen zijn beschikbaar in je studentenportaal. Controleer je scores en feedback.",
                "https://www.google.com/",
                NotificationTopics.Default.Name),
            new Notification(
                "Bibliotheek: aangepaste openingsuren",
                "De bibliotheek heeft deze week gewijzigde openingsuren vanwege inventarisatie. Kijk op de bibliotheekpagina voor actuele tijden.",
                "https://www.google.com/", NotificationTopics.Default.Name),
            new Notification(
                "Veiligheidsmelding: gladde paden",
                "Let op bij het betreden van de campus: door vorst kunnen paden lokaal glad zijn. Draag geschikte schoenen en volg de aanwijzingen.",
                "https://www.google.com/", NotificationTopics.Default.Name),
            new Notification(
                "Evenement: Carrièrebeurs volgende week",
                "De jaarlijkse carrièrebeurs vindt volgende week plaats in de aula. Studenten worden aangemoedigd om zich in te schrijven en CV's mee te brengen.",
                "https://www.google.com/", NotificationTopics.NewsOfEvent.Name),
            new Notification(
                "Enquête: studenten tevredenheid",
                "Help ons verbeteren — vul de korte tevredenheidsenquête in. Het invullen duurt minder dan 5 minuten.",
                "https://www.google.com/", NotificationTopics.Default.Name),
            new Notification(
                "Nieuw weekmenu beschikbaar",
                "Het nieuwe weekmenu van de resto's is gepubliceerd. Bekijk de opties en allergeneninformatie via de menu-pagina.",
                "https://www.google.com/", NotificationTopics.Default.Name),
            new Notification(
                "Aanmelding open: workshops en bijscholing",
                "Inschrijvingen voor de komende workshops en bijscholingen zijn geopend. Plaatsen zijn beperkt — schrijf je snel in.",
                "https://www.google.com/", NotificationTopics.NewsOfEvent.Name),
            new Notification(
                "[BP] - Infosessie bachelorproef als jaarvak",
                @"
                    <p>Beste student,</p>

                    <p>
                      Ik wil je bij deze van harte uitnodigen voor de infosessie over de bachelorproef als jaarvak. 
                      Ik geef de sessie 2x:
                    </p>

                    <ul>
                      <li>'s avonds op <strong style=""color:black;"">maandag 6 oktober</strong>, tussen 18.30-19.30u, via Teams:<br>
                        <a style=""color:var(--secondary-color); text-decoration:underline;"" href=""https://teams.microsoft.com/dl/launcher/launcher.html?url=%2F_%23%2Fl%2Fmeetup-join%2F19%3Ameeting_MTMyZTE2YTctZDBiZS00OGNjLTg3MWQtYjI5MjBiNTAyNzUx%40thread.v2%2F0%3Fcontext%3D%257b%2522Tid%2522%253a%25225cf7310e-091a-4bc5-acd7-26c721d4cccd%2522%252c%2522Oid%2522%253a%252201c9723d-1796-474d-b7ee-4a890eefe12e%2522%257d%26anon%3Dtrue&type=meetup-join&deeplinkId=8dcc7ef2-ec3a-41cf-9b5d-446980bed859&directDl=true&msLaunch=true&enableMobilePage=true&suppressPrompt=true"">
                          Deelnemen via Microsoft Teams
                        </a>
                      </li>

                      <li>over de middag op <strong style=""color:black;"">donderdag 09 oktober</strong>, tussen 12.30-13.30u, fysiek op de campus in de BCON (B.0.010).</li>
                    </ul>

                    <p>
                      In de sessie overloop ik de tijdslijn en verwachtingen rond de bachelorproef en benadruk ik welke aspecten bijzonder belangrijk zijn. 
                      Ik beantwoord ook alle vragen die jullie eventueel hebben. Voor wie niet aanwezig kan zijn, maak ik een opname die ik daarna via Chamilo zal delen.
                    </p>

                    <p>
                      Hebben jullie nu al vragen? Onder <strong style=""color:black;"">documenten &gt; BP als jaarvak</strong> vinden jullie het document met de richtlijnen terug:<br>
                      <a style=""color:var(--secondary-color); text-decoration:underline;"" href=""https://chamilo.hogent.be/index.php?go=CourseViewer&application=Chamilo%5CApplication%5CWeblcms&course=70063&tool=Document&publication_category=356623&browser=Table&tool_action=Browser"">
                        Documenten – BP als jaarvak (Chamilo)
                      </a>
                    </p>

                    <p>
                      De <strong style=""color:black;"">tijdslijn voor de BP als jaarvak</strong> kan je eveneens daar vinden:<br>
                      <a style=""color:var(--secondary-color); text-decoration:underline;"" href=""https://chamilo.hogent.be/index.php?go=CourseViewer&application=Chamilo%5CApplication%5CWeblcms&course=70063&tool=Document&publication_category=356623&browser=Table&tool_action=Viewer&publication=2576232"">
                        Tijdslijn BP als jaarvak (Chamilo)
                      </a>
                    </p>

                    <p>
                      De deadlines kan je meteen in je agenda plaatsen! Als je een vraag hebt waarop je het antwoord niet in de tijdslijn of richtlijnen kan vinden 
                      en de vraag niet kan wachten tot de 3de lesweek, mag je me steeds mailen.
                    </p>

                    <p>Tot dan!</p>

                    <p>
                      <strong style=""color:black;"">Lena De Mol</strong><br>
                      Bachelorproefcoördinator Toegepaste Informatica
                    </p>
                    "
                ,
                "https://chamilo.hogent.be/index.php?go=CourseViewer&application=Chamilo%5CApplication%5CWeblcms&course=70063&tool=Announcement&publication_category=356915&browser=List&tool_action=Viewer&publication=2582134",
                NotificationTopics.Aankondiging.Name),

            new Notification(
                "Argumentative presentation - Pick your topic",
                @"
                    <p>Dear students</p>

                    <p>
                      The topics you can choose from for the argumentative presentation (20%) were shared with you under 
                      <a style=""color:var(--secondary-color); text-decoration:underline;"" href=""https://chamilo.hogent.be/index.php?go=CourseViewer&application=Chamilo%5CApplication%5CWeblcms&course=68394&tool=Document&publication_category=358652&browser=Table&tool_action=Viewer&publication=2606189"">
                        documents &gt; assessment
                      </a>.
                      Please pick one that resonates with your interests in computer engineering and challenges you to think critically.
                      Each topic is designed to spark debate and encourage you to explore emerging technologies, ethical dilemmas and/or the impact on society.
                    </p>

                    <h3>Registration for topic</h3>

                    <p>
                      Register your choice in 
                      <a style=""color:var(--secondary-color); text-decoration:underline;"" href=""https://chamilo.hogent.be/index.php?go=CourseViewer&application=Chamilo%5CApplication%5CWeblcms&course=68394&tool=Document&publication_category=358652&browser=Table&tool_action=Viewer&publication=2606191"">
                        this Microsoft Form
                      </a>
                      before <strong style=""color:black;"">27/10, midnight</strong>.  
                      If you miss the deadline, a topic will be assigned to you by the lecturer.
                    </p>

                    <p>
                      In the course of <strong style=""color:black;"">week 6</strong>, the lecturer will share a document with your topic and the date on which you have to hold your presentation.
                      More information regarding the presentation and its prerequisites can be found in the document under assessment (see above).
                    </p>

                    <p>Kind regards</p>

                    <p>
                      <strong style=""color:black;"">Femke Cornette</strong>
                    </p>
                    ",
                "https://chamilo.hogent.be/index.php?go=CourseViewer&application=Chamilo%5CApplication%5CWeblcms&course=68394&tool=Announcement&publication_category=359484&browser=List&tool_action=Viewer&publication=2609057",
                NotificationTopics.Aankondiging.Name),
            new Notification(
                "[BP] - promoten werden toegewezen",
                @"
                    <p>Beste student</p>

                    <p>
                      Eerder vandaag verstuurde ik via mailmerge een bericht naar iedereen die de form voor het indienen van het onderzoeksvoorstel tegen 
                      <strong style=""color:black;"">12/11</strong> correct instuurde, met daarin de naam van de collega die als promotor zal optreden.
                      Het is nu aan jou om contact op te nemen met je promotor (via e-mail) en een afspraak in te plannen om je voorstel mondeling toe te lichten.
                      Tegen <strong style=""color:black;"">12/12</strong> moet je een volledig uitgeschreven onderzoeksvoorstel indienen.
                      De opdracht hiervoor kan je op Chamilo vinden, inclusief een beschrijving van wat verwacht wordt:
                      <br>
                      <a style=""color:var(--secondary-color); text-decoration:underline;"" href=""https://chamilo.hogent.be/index.php?go=CourseViewer&application=Chamilo%5CApplication%5CWeblcms&course=70063&tool=Assignment&publication_category=356632&browser=Table&tool_action=Display&publication=2605485#assignment"">
                        Opdracht onderzoeksvoorstel (Chamilo)
                      </a>
                    </p>

                    <p>
                      Volg je de specialisatie <strong style=""color:black;"">mainframe</strong>? Dan werd er nog geen promotor toegewezen, dat komt later.
                    </p>

                    <p>
                      Kreeg je geen mail, maar denk je het formulier wel correct te hebben ingediend? 
                      Neem dan via e-mail contact met me op.
                    </p>

                    <p>Veel succes!</p>

                    <p>
                      Met vriendelijke groeten<br>
                      <strong style=""color:black;"">Lena De Mol</strong>
                    </p>
                    ",
                "https://www.google.com/",
                NotificationTopics.Aankondiging.Name),
            new Notification(
                "Gaslek op HOGENT Campus Schoonmeersen",
                "Er is een gaslek vastgesteld op de HOGENT Campus Schoonmeersen. Voor uw veiligheid verzoeken wij u dringend het gebouw te verlaten en de instructies van het noodpersoneel op te volgen.",
                "https://www.google.com/",
                NotificationTopics.Warning.Name
            )
        };

        await dbContext.Notifications.AddRangeAsync(notificationsList);
    }

    private async Task AssignNotificationsToStudentAsync(Student student)
    {
        var allNotifications = await dbContext.Notifications
            .OrderBy(n => n.Id)
            .ToListAsync();
        var id = student.Id;

        var takeTrue = Math.Min(11, allNotifications.Count);
        var takeFalse = Math.Min(3, Math.Max(0, allNotifications.Count - takeTrue));

        var trueNotifications = allNotifications.Take(takeTrue);
        var falseNotifications = allNotifications.Skip(takeTrue).Take(takeFalse);

        foreach (var n in trueNotifications)
        {
            dbContext.UserNotifications.Add(new UserNotification(id, n.Id, true));
        }

        foreach (var n in falseNotifications)
        {
            dbContext.UserNotifications.Add(new UserNotification(id, n.Id));
        }
    }

    private async Task RolesAsync()
    {
        if (dbContext.Roles.Any()) return;
        await roleManager.CreateAsync(new IdentityRole("Administrator"));
        await roleManager.CreateAsync(new IdentityRole("Secretary"));
        await roleManager.CreateAsync(new IdentityRole("Student"));
        await roleManager.CreateAsync(new IdentityRole("Employee"));
    }

    private async Task DepartmentsAsync()
    {
        if (dbContext.Departments.Any()) return;

        dbContext.Departments.AddRange(
            new Department
            {
                Name = "Bedrijf en Organisatie",
                Description = "Programma's in bedrijfskunde, financiën, marketing en organisatorische ontwikkeling",
                DepartmentType = DepartmentType.Department
            },
            new Department
            {
                Name = "Bio- en Industriële Technologie",
                Description = "Studies in biotechnologie, agrowetenschappen, chemie en industriële technologie",
                DepartmentType = DepartmentType.Department
            },
            new Department
            {
                Name = "Gezondheidszorg",
                Description = "Opleidingen in verpleegkunde, logopedie, audiologie en andere gezondheidsberoepen",
                DepartmentType = DepartmentType.Department
            },
            new Department
            {
                Name = "IT en Digitale Innovatie",
                Description = "Opleidingen in toegepaste informatica, digitaal ontwerp en IT-beheer",
                DepartmentType = DepartmentType.Department
            },
            new Department
            {
                Name = "Lerarenopleiding",
                Description = "Opleidingen voor toekomstige leraren in secundair en basisonderwijs",
                DepartmentType = DepartmentType.Department
            },
            new Department
            {
                Name = "Omgeving",
                Description = "Focus op vastgoed, houttechnologie, landschapsarchitectuur en milieubeheer",
                DepartmentType = DepartmentType.Department
            },
            new Department
            {
                Name = "Sociaal Werk",
                Description = "Programma's in sociaal werk, pedagogiek en gemeenschapsontwikkeling",
                DepartmentType = DepartmentType.Department
            },
            new Department
            {
                Name = "GO5", Description = "School voor associate degrees in diverse praktische vakgebieden",
                DepartmentType = DepartmentType.Department
            },
            new Department
            {
                Name = "KASK & Conservatorium",
                Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design",
                DepartmentType = DepartmentType.Department
            },
            new Department
            {
                Name = "Studentenraad",
                Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design",
                DepartmentType = DepartmentType.Other
            },
            new Department
            {
                Name = "Sportdienst",
                Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design",
                DepartmentType = DepartmentType.Other
            },
            new Department
            {
                Name = "Algemene directie",
                Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design",
                DepartmentType = DepartmentType.Management
            },
            new Department
            {
                Name = "Directie Onderwijsaangelegenheden",
                Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design",
                DepartmentType = DepartmentType.Management
            },
            new Department
            {
                Name = "Directie Onderzoeksaangelegenheden",
                Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design",
                DepartmentType = DepartmentType.Management
            },
            new Department
            {
                Name = "Directie Financiën en Personeel",
                Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design",
                DepartmentType = DepartmentType.Management
            },
            new Department
            {
                Name = "Directie IT",
                Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design",
                DepartmentType = DepartmentType.Management
            },
            new Department
            {
                Name = "Directie Infrasturctuur en Facilitair Beheer",
                Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design",
                DepartmentType = DepartmentType.Management
            },
            new Department
            {
                Name = "Directie Studentenvoorzieningen",
                Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design",
                DepartmentType = DepartmentType.Management
            },
            new Department
            {
                Name = "Directie Communicatie",
                Description = "Kunstschool met programma's in beeldende kunsten, muziek, drama en design",
                DepartmentType = DepartmentType.Management
            }
        );

        await dbContext.SaveChangesAsync();
    }

    private async Task ClassGroupsAsync()
    {
        if (dbContext.Classgroups.Any()) return;

        var classgroups = new[]
        {
            new ClassGroup { Name = "3A1" },
            new ClassGroup { Name = "3A2" },
            new ClassGroup { Name = "3B1" },
            new ClassGroup { Name = "3B2" },
        };

        await dbContext.Classgroups.AddRangeAsync(classgroups);
    }

    private async Task UsersAsync()
    {
        if (dbContext.Users.Any())
            return;

        await dbContext.Roles.ToListAsync();


        // Default departments to assign
        // =============================
        var defaultDepartment = await dbContext.Departments
            .FirstAsync(d => d.Name.Contains("IT"));
        var defaultDepartment2 = await dbContext.Departments
            .FirstAsync(d => d.Name.Contains("Bedrijf"));
        var defaultDepartment3 = await dbContext.Departments
            .FirstAsync(d => d.Name.Contains("Bio"));


        // Create accounts
        // ===============

        await CreateUserAsync("admin@example.com", "Administrator");
        await CreateUserAsync("secretary@example.com", "Secretary");
        var managerAccount1 = await CreateUserAsync("rudi.madalijns@hogent.be", "Employee");
        var managerAccount2 = await CreateUserAsync("christel.meert@hogent.be", "Employee");
        var managerAccount3 = await CreateUserAsync("liesbeth.vancoppenolle@hogent.be", "Employee");
        var studentAccount1 = await CreateUserAsync("jane.doe@student.hogent.be", "Student");
        var studentAccount2 = await CreateUserAsync("gert.vanderstenen@student.hogent.be", "Student");
        var studentAccount3 = await CreateUserAsync("student1@student.hogent.be", "Student");
        var studentAccount4 = await CreateUserAsync("student2@student.hogent.be", "Student");
        var studentAccount5 = await CreateUserAsync("student3@student.hogent.be", "Student");
        var studentAccount6 = await CreateUserAsync("student4@student.hogent.be", "Student");
        var studentAccount99 = await CreateUserAsync("stefaan1@student.hogent.be", "Student");
        var teacherAcc1 = await CreateUserAsync("gilles.blondeel@hogent.be", "Employee");
        var teacherAcc2 = await CreateUserAsync("benjamin.vertonghen@hogent.be", "Employee");
        var teacherAcc3 = await CreateUserAsync("chloe.deleenheer@hogent.be", "Employee");
        var teacherAcc4 = await CreateUserAsync("gertjan.bosteels@hogent.be", "Employee");
        var teacherAcc5 = await CreateUserAsync("jan.willem@hogent.be", "Employee");
        var teacherAcc6 = await CreateUserAsync("pieter.vanderhelst@hogent.be", "Employee");
        var teacherAcc7 = await CreateUserAsync("anneleen.bekkens@hogent.be", "Employee");
        var teacherAcc8 = await CreateUserAsync("martine.vanaudenrode@hogent.be", "Employee");
        var teacherAcc9 = await CreateUserAsync("olivier.rosseel@hogent.be", "Employee");
        var teacherAcc10 = await CreateUserAsync("sebastiaan.labijn@hogent.be", "Employee");
        var managerAccount4 = await CreateUserAsync("chantal.teerlinck@hogent.be", "Employee");
        var managerAccount5 = await CreateUserAsync("lincy.vantwembeke@hogent.be", "Employee");
        var managerAccount6 = await CreateUserAsync("mieke.paelinck@hogent.be", "Employee");
        var managerAccount7 = await CreateUserAsync("claudia.claes@hogent.be", "Employee");
        var managerAccount8 = await CreateUserAsync("jan.vandenberghe@hogent.be", "Employee");
        var managerAccount9 = await CreateUserAsync("filip.rathe@hogent.be", "Employee");
        var managerAccount10 = await CreateUserAsync("bengt.verbeeck@hogent.be", "Employee");
        var managerAccount11 = await CreateUserAsync("yves.ronsse@hogent.be", "Employee");
        var managerAccount12 = await CreateUserAsync("bart.kimpe@hogent.be", "Employee");
        var managerAccount13 = await CreateUserAsync("robin.stevens@hogent.be", "Employee");
        var managerAccount14 = await CreateUserAsync("els.stuyven@hogent.be", "Employee");
        var managerAccount15 = await CreateUserAsync("sofie.jaques@hogent.be", "Employee");
        var managerAccount16 = await CreateUserAsync("bart.derouck@hogent.be", "Employee");
        var managerAccount17 = await CreateUserAsync("rudi.devierman@hogent.be", "Employee");
        var managerAccount18 = await CreateUserAsync("sebastien.malfait@hogent.be", "Employee");
        var managerAccount19 = await CreateUserAsync("maaike.teirlinck@hogent.be", "Employee");
        var managerAccount20 = await CreateUserAsync("isabelle.claeys@hogent.be", "Employee");


        // Create specific user entities
        // =============================

        var students = new[]
        {
            new Student
            {
                StudentNumber = "S12345",
                AccountId = studentAccount1.Id,
                Email = new EmailAddress("jane.doe@student.hogent.be"),
                Firstname = "Jane",
                Lastname = "Doe",
                Department = defaultDepartment,
                Birthdate = new DateTime(2007, 9, 1),
                PreferedCampus = "Schoonmeersen"
            },

            new Student
            {
                StudentNumber = "S12346",
                AccountId = studentAccount2.Id,
                Email = new EmailAddress("gert.vanderstenen@student.hogent.be"),
                Firstname = "Gert",
                Lastname = "Van Der Stenen",
                Department = defaultDepartment,
                Birthdate = new DateTime(2001, 8, 29),
                PreferedCampus = "Schoonmeersen"
            },
            new Student
            {
                StudentNumber = "S12347",
                AccountId = studentAccount3.Id,
                Email = new EmailAddress("student1@student.hogent.be"),
                Firstname = "test",
                Lastname = "test",
                Department = defaultDepartment,
                Birthdate = new DateTime(2007, 6, 1),
                PreferedCampus = "Schoonmeersen"
            },
            new Student
            {
                StudentNumber = "S12348",
                AccountId = studentAccount4.Id,
                Email = new EmailAddress("student2@student.hogent.be"),
                Firstname = "test",
                Lastname = "test",
                Department = defaultDepartment,
                Birthdate = new DateTime(2007, 4, 1),
                PreferedCampus = "Schoonmeersen"
            },
            new Student
            {
                StudentNumber = "S12349",
                AccountId = studentAccount5.Id,
                Email = new EmailAddress("student3@student.hogent.be"),
                Firstname = "test",
                Lastname = "test",
                Department = defaultDepartment,
                Birthdate = new DateTime(2002, 5, 1),
                PreferedCampus = "Schoonmeersen"
            },
            new Student
            {
                StudentNumber = "S12350",
                AccountId = studentAccount6.Id,
                Email = new EmailAddress("student4@student.hogent.be"),
                Firstname = "test",
                Lastname = "test",
                Department = defaultDepartment,
                Birthdate = new DateTime(2005, 8, 1),
                PreferedCampus = "Schoonmeersen"
            },
            new Student
            {
                StudentNumber = "S12399",
                AccountId = studentAccount99.Id,
                Email = new EmailAddress("stefaan1@student.hogent.be"),
                Firstname = "Stefaan",
                Lastname = "Vanbillemont",
                Department = defaultDepartment,
                Birthdate = new DateTime(2000, 8, 1),
                PreferedCampus = "Schoonmeersen",
            },
        };
        dbContext.Students.AddRange(students);

        var teachers = new[]
        {
            new Teacher
            {
                Firstname = "Gilles",
                Lastname = "Blondeel",
                AccountId = teacherAcc1.Id,
                Email = new EmailAddress(teacherAcc1.Email ?? string.Empty),
                Department = defaultDepartment,
                Birthdate = new DateTime(2025, 12, 11)
            },
            new Teacher
            {
                Firstname = "Benjamin",
                Lastname = "Vertonghen",
                AccountId = teacherAcc2.Id,
                Email = new EmailAddress(teacherAcc2.Email ?? string.Empty),
                Department = defaultDepartment,
                Birthdate = new DateTime(2025, 12, 11)
            },
            new Teacher
            {
                Firstname = "Chloé",
                Lastname = "De Leenheer",
                AccountId = teacherAcc3.Id,
                Email = new EmailAddress(teacherAcc3.Email ?? string.Empty),
                Department = defaultDepartment,
                Birthdate = new DateTime(2025, 12, 11)
            },
            new Teacher
            {
                Firstname = "Gertjan",
                Lastname = "Bosteels",
                AccountId = teacherAcc4.Id,
                Email = new EmailAddress(teacherAcc4.Email ?? string.Empty),
                Department = defaultDepartment,
                Birthdate = new DateTime(2025, 12, 11)
            },
            new Teacher
            {
                Firstname = "Jan",
                Lastname = "Willem",
                AccountId = teacherAcc5.Id,
                Email = new EmailAddress(teacherAcc5.Email ?? string.Empty),
                Department = defaultDepartment,
                Birthdate = new DateTime(2025, 12, 11)
            },
            new Teacher
            {
                Firstname = "Pieter",
                Lastname = "Van Der Helst",
                AccountId = teacherAcc6.Id,
                Email = new EmailAddress(teacherAcc6.Email ?? string.Empty),
                Department = defaultDepartment,
                Birthdate = new DateTime(2025, 12, 11)
            },
            new Teacher
            {
                Firstname = "Anneleen",
                Lastname = "Bekkens",
                AccountId = teacherAcc7.Id,
                Email = new EmailAddress(teacherAcc7.Email ?? string.Empty),
                Department = defaultDepartment,
                Birthdate = new DateTime(2025, 12, 11)
            },
            new Teacher
            {
                Firstname = "Martine",
                Lastname = "Van Audenrode",
                AccountId = teacherAcc8.Id,
                Email = new EmailAddress(teacherAcc8.Email ?? string.Empty),
                Department = defaultDepartment,
                Birthdate = new DateTime(2025, 12, 11)
            },
            new Teacher
            {
                Firstname = "Olivier",
                Lastname = "Rosseel",
                AccountId = teacherAcc9.Id,
                Email = new EmailAddress(teacherAcc9.Email ?? string.Empty),
                Department = defaultDepartment,
                Birthdate = new DateTime(2025, 12, 11),
                IsAbsent = true
            },
            new Teacher
            {
                Firstname = "Sebastiaan",
                Lastname = "Labijn",
                AccountId = teacherAcc10.Id,
                Email = new EmailAddress(teacherAcc10.Email ?? string.Empty),
                Department = defaultDepartment,
                Birthdate = new DateTime(2025, 12, 11)
            },
        };
        dbContext.Teachers.AddRange(teachers);

        List<Department> departments = dbContext.Departments.ToList();
        var employees = new[]
        {
            new Employee
            {
                Employeenumber = "E001",
                AccountId = managerAccount1.Id,
                Email = new EmailAddress("rudi.madalijns@hogent.be"),
                Firstname = "Rudi",
                Lastname = "Madalijns",
                Department = departments.FirstOrDefault(d => d.Name == "Bedrijf en Organisatie") ?? defaultDepartment,
                Birthdate = new DateTime(1970, 1, 1),
                Title = "Departementshoofd"
            },
            new Employee
            {
                Employeenumber = "E002",
                AccountId = managerAccount2.Id,
                Email = new EmailAddress("christel.meert@hogent.be"),
                Firstname = "Christel",
                Lastname = "Meert",
                Department = departments.FirstOrDefault(d => d.Name == "Biowetenschappen en Industriële Technologie") ??
                             defaultDepartment,
                Birthdate = new DateTime(1975, 3, 22),
                Title = "Departementshoofd"
            },
            new Employee
            {
                Employeenumber = "E003",
                AccountId = managerAccount3.Id,
                Email = new EmailAddress("liesbeth.vancoppenolle@hogent.be"),
                Firstname = "Liesbeth",
                Lastname = "Van Coppenolle",
                Department = departments.FirstOrDefault(d => d.Name == "Gezondheidszorg") ?? defaultDepartment,
                Birthdate = new DateTime(1982, 9, 10),
                Title = "Departementshoofd"
            },
            new Employee
            {
                Employeenumber = "E004",
                AccountId = managerAccount4.Id,
                Email = new EmailAddress("chantal.teerlinck@hogent.be"),
                Firstname = "Chantal",
                Lastname = "Teerlinck",
                Department = departments.FirstOrDefault(d => d.Name == "IT en Digitale Innovatie") ?? defaultDepartment,
                Birthdate = new DateTime(1978, 11, 5),
                Title = "Departementshoofd"
            },
            new Employee
            {
                Employeenumber = "E005",
                AccountId = managerAccount5.Id,
                Email = new EmailAddress("lincy.vantwembeke@hogent.be"),
                Firstname = "Lincy",
                Lastname = "Van Twembeke",
                Department = departments.FirstOrDefault(d => d.Name == "Lerarenopleiding") ?? defaultDepartment,
                Birthdate = new DateTime(1985, 4, 18),
                Title = "Departementshoofd"
            },
            new Employee
            {
                Employeenumber = "E006",
                AccountId = managerAccount6.Id,
                Email = new EmailAddress("mieke.paelinck@hogent.be"),
                Firstname = "Mieke",
                Lastname = "Paelinck",
                Department = departments.FirstOrDefault(d => d.Name == "Omgeving") ?? defaultDepartment,
                Birthdate = new DateTime(1972, 7, 29),
                Title = "Departementshoofd"
            },
            new Employee
            {
                Employeenumber = "E007",
                AccountId = managerAccount7.Id,
                Email = new EmailAddress("claudia.claes@hogent.be"),
                Firstname = "Claudia",
                Lastname = "Claes",
                Department = departments.FirstOrDefault(d => d.Name == "Sociaal-Agogisch Werk") ?? defaultDepartment,
                Birthdate = new DateTime(1988, 1, 12),
                Title = "Departementshoofd"
            },
            new Employee
            {
                Employeenumber = "E008",
                AccountId = managerAccount8.Id,
                Email = new EmailAddress("jan.vandenberghe@hogent.be"),
                Firstname = "Jan",
                Lastname = "Vanden Berghe",
                Department = departments.FirstOrDefault(d => d.Name == "GO5") ?? defaultDepartment,
                Birthdate = new DateTime(1969, 10, 3),
                Title = "Departementshoofd"
            },
            new Employee
            {
                Employeenumber = "E009",
                AccountId = managerAccount9.Id,
                Email = new EmailAddress("filip.rathe@hogent.be"),
                Firstname = "Filip",
                Lastname = "Rathé",
                Department = departments.FirstOrDefault(d => d.Name == "KASK & Conservatorium") ?? defaultDepartment,
                Birthdate = new DateTime(1976, 5, 27),
                Title = "Decaan"
            },
            new Employee
            {
                Employeenumber = "E010",
                AccountId = managerAccount10.Id,
                Email = new EmailAddress("bengt.verbeeck@hogent.be"),
                Firstname = "Bengt",
                Lastname = "Verbeeck",
                Department = departments.FirstOrDefault(d => d.Name == "Algemene directie") ?? defaultDepartment,
                Birthdate = new DateTime(1983, 2, 14),
                Title = "Directeur Algemene directie"
            },
            new Employee
            {
                Employeenumber = "E011",
                AccountId = managerAccount11.Id,
                Email = new EmailAddress("yves.ronsse@hogent.be"),
                Firstname = "Yves",
                Lastname = "Ronsse",
                Department = departments.FirstOrDefault(d => d.Name == "Algemene directie") ?? defaultDepartment,
                Birthdate = new DateTime(1971, 8, 9),
                Title = "Adjunct Directeur Algemene directie"
            },
            new Employee
            {
                Employeenumber = "E012",
                AccountId = managerAccount12.Id,
                Email = new EmailAddress("bart.kimpe@hogent.be"),
                Firstname = "Bart",
                Lastname = "Kimpe",
                Department = departments.FirstOrDefault(d => d.Name == "Algemene directie") ?? defaultDepartment,
                Birthdate = new DateTime(1986, 12, 21),
                Title = "Diensthoofd Interne Audit"
            },
            new Employee
            {
                Employeenumber = "E013",
                AccountId = managerAccount13.Id,
                Email = new EmailAddress("robin.stevens@hogent.be"),
                Firstname = "Robin",
                Lastname = "Stevens",
                Department = departments.FirstOrDefault(d => d.Name == "Directie Onderwijsaangelegenheden") ??
                             defaultDepartment,
                Birthdate = new DateTime(1979, 6, 7),
                Title = "Directeur Onderwijsaangelegenheden"
            },
            new Employee
            {
                Employeenumber = "E014",
                AccountId = managerAccount14.Id,
                Email = new EmailAddress("els.stuyven@hogent.be"),
                Firstname = "Els",
                Lastname = "Stuyven",
                Department = departments.FirstOrDefault(d => d.Name == "Directie Onderzoeksaangelegenheden") ??
                             defaultDepartment,
                Birthdate = new DateTime(1984, 3, 30),
                Title = "Directeur Onderzoeksaangelegenheden"
            },
            new Employee
            {
                Employeenumber = "E015",
                AccountId = managerAccount15.Id,
                Email = new EmailAddress("sofie.jaques@hogent.be"),
                Firstname = "Sofie",
                Lastname = "Jaques",
                Department = departments.FirstOrDefault(d => d.Name == "Directie Financiën en Personeel") ??
                             defaultDepartment,
                Birthdate = new DateTime(1974, 11, 16),
                Title = "Directeur Financiën en Personeel"
            },
            new Employee
            {
                Employeenumber = "E016",
                AccountId = managerAccount16.Id,
                Email = new EmailAddress("bart.derouck@hogent.be"),
                Firstname = "Bart",
                Lastname = "De Rouck",
                Department = departments.FirstOrDefault(d => d.Name == "Directie IT") ?? defaultDepartment,
                Birthdate = new DateTime(1981, 9, 25),
                Title = "Directeur IT"
            },
            new Employee
            {
                Employeenumber = "E017",
                AccountId = managerAccount17.Id,
                Email = new EmailAddress("rudi.devierman@hogent.be"),
                Firstname = "Rudi",
                Lastname = "De Vierman",
                Department =
                    departments.FirstOrDefault(d => d.Name == "Directie Infrastructuur en Facilitair Beheer") ??
                    defaultDepartment,
                Birthdate = new DateTime(1968, 4, 8),
                Title = "Directeur Infrastructuur en Facilitair Beheer"
            },
            new Employee
            {
                Employeenumber = "E018",
                AccountId = managerAccount18.Id,
                Email = new EmailAddress("sebastien.malfait@hogent.be"),
                Firstname = "Sebastien",
                Lastname = "Malfait",
                Department = departments.FirstOrDefault(d => d.Name == "Directie Studentenvoorzieningen") ??
                             defaultDepartment,
                Birthdate = new DateTime(1987, 7, 19),
                Title = "Directeur Studentenvoorzieningen"
            },
            new Employee
            {
                Employeenumber = "E019",
                AccountId = managerAccount19.Id,
                Email = new EmailAddress("maaike.teirlinck@hogent.be"),
                Firstname = "Maaike",
                Lastname = "Teirlinck",
                Department = departments.FirstOrDefault(d => d.Name == "Directie Communicatie") ?? defaultDepartment,
                Birthdate = new DateTime(1977, 10, 11),
                Title = "Diensthoofd Communicatie"
            },
            new Employee
            {
                Employeenumber = "E020",
                AccountId = managerAccount20.Id,
                Email = new EmailAddress("isabelle.claeys@hogent.be"),
                Firstname = "Isabelle",
                Lastname = "Claeys",
                Department = departments.FirstOrDefault(d => d.Name == "Directie Communicatie") ?? defaultDepartment,
                Birthdate = new DateTime(1989, 5, 4),
                Title = "Communicatiemanager"
            }
        };
        dbContext.Employees.AddRange(employees);


        await dbContext.SaveChangesAsync();

        // Add default shortcuts to users
        // ==============================
        await AssignDefaultShortcutsToStudentAsync(students[0]);
        await AssignDefaultShortcutsToStudentAsync(students[1]);
        await AssignDefaultShortcutsToStudentAsync(students[2]);
        await AssignDefaultShortcutsToStudentAsync(students[3]);
        await AssignDefaultShortcutsToStudentAsync(students[4]);
        await AssignDefaultShortcutsToStudentAsync(students[5]);
        await AssignDefaultShortcutsToStudentAsync(students[6]);
        // Add default notifications to users
        // ==============================
        await AssignNotificationsToStudentAsync(students[0]);
        await AssignNotificationsToStudentAsync(students[1]);
        await AssignNotificationsToStudentAsync(students[2]);
        await AssignNotificationsToStudentAsync(students[3]);
        await AssignNotificationsToStudentAsync(students[4]);
        await AssignNotificationsToStudentAsync(students[5]);
        await AssignNotificationsToStudentAsync(students[6]);
        await dbContext.SaveChangesAsync();
    }

    private async Task<IdentityUser> CreateUserAsync(string email, string role)
    {
        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(user, PasswordDefault);

        await userManager.AddToRoleAsync(user, role);

        return user;
    }

    private async Task ShortcutsAsync()
    {
        if (dbContext.Shortcuts.Any()) return;

        var roostersEnKalenders = new[]
        {
            new Shortcut("TimeEdit", ShortcutType.SchedulesAndCalendars, "fa-solid fa-calendar-days", "",
                "https://roosters.hogent.be/", true),
            new Shortcut("Examenrooster", ShortcutType.SchedulesAndCalendars, "fa-solid fa-calendar-check", "",
                "https://roosters.hogent.be/", false),
            new Shortcut("Academische kalender", ShortcutType.SchedulesAndCalendars, "fa-solid fa-calendar", "",
                "https://www.hogent.be/student/een-vlotte-start/academische-kalender/", false),
            new Shortcut("Weekmenu", ShortcutType.SchedulesAndCalendars, "fa-solid fa-utensils", "", "resto", true)
        };

        var studiediensten = new[]
        {
            new Shortcut("Orion", ShortcutType.Studyservices, "fa-solid fa-school", "",
                "https://chamilo.hogent.be/index.php?application=Chamilo\\Core\\Home", true),
            new Shortcut("Wallie", ShortcutType.Studyservices, "fa-solid fa-circle-info", "",
                "https://hogent.sharepoint.com/sites/IntranetStudenten", true),
            new Shortcut("IBamaflex", ShortcutType.Studyservices, "", "IB", "https://ibamaflex.hogent.be/", true),
            new Shortcut("Ans", ShortcutType.Studyservices, "fa-regular fa-file-lines", "",
                "https://ans.app/users/sign_in", false),
            new Shortcut("exam.hogent.be", ShortcutType.Studyservices, "fa-solid fa-clipboard-list", "",
                "https://exam.hogent.be/", false),
            new Shortcut("Academic Software", ShortcutType.Studyservices, "fa-solid fa-computer", "",
                "https://platform-api.academicsoftware.com/login/login?userName=@hogent.be&language=nl", false),
        };

        var communicatieEnSoftware = new[]
        {
            new Shortcut("Teams", ShortcutType.CommunicationAndSoftware, "fa-solid fa-users", "",
                "https://teams.microsoft.com/l/team", false),
            new Shortcut("Panopto", ShortcutType.CommunicationAndSoftware, "fa-solid fa-circle-play", "",
                "https://hogent.cloud.panopto.eu/", false),
            new Shortcut("E-mail", ShortcutType.CommunicationAndSoftware, "fa-solid fa-envelope", "",
                "https://outlook.com/owa/hogent.be", false),
            new Shortcut("OneDrive", ShortcutType.CommunicationAndSoftware, "fa-brands fa-microsoft", "",
                "https://login.microsoftonline.com/login.srf?wa=wsignin1%2E0&rver=6%2E1%2E6206%2E0&wreply=https%3A%2F%2Fhogent-my.sharepoint.com%2F&whr=hogent.be",
                false),
            new Shortcut("Word", ShortcutType.CommunicationAndSoftware, "fa-solid fa-file-word", "",
                "https://login.microsoftonline.com/login.srf?wa=wsignin1%2E0&rver=6%2E1%2E6206%2E0&wreply=https%3A%2F%2Foffice.live.com%2Fstart%2FWord.aspx%3Fauth%3D2&whr=hogent.be",
                false),
            new Shortcut("Excel", ShortcutType.CommunicationAndSoftware, "fa-solid fa-file-excel", "",
                "https://login.microsoftonline.com/login.srf?wa=wsignin1%2E0&rver=6%2E1%2E6206%2E0&wreply=https%3A%2F%2Foffice.live.com%2Fstart%2FExcel.aspx%3Fauth%3D2&whr=hogent.be",
                false),
            new Shortcut("PowerPoint", ShortcutType.CommunicationAndSoftware, "fa-solid fa-file-powerpoint", "",
                "https://login.microsoftonline.com/login.srf?wa=wsignin1%2E0&rver=6%2E1%2E6206%2E0&wreply=https%3A%2F%2Foffice.live.com%2Fstart%2FPowerPoint.aspx%3Fauth%3D2&whr=hogent.be",
                false),
            new Shortcut("OneNote", ShortcutType.CommunicationAndSoftware, "fa-solid fa-note-sticky", "",
                "https://www.onenote.com/notebooks?auth=2", false),
            new Shortcut("Forms", ShortcutType.CommunicationAndSoftware, "fa-regular fa-file-lines", "",
                "https://forms.office.com/Pages/DesignPage.aspx?auth_pvr=OrgId&auth_upn=@hogent.be", false),
        };

        var ondersteuningEnIt = new[]
        {
            new Shortcut("IT helpdesk", ShortcutType.SupportAndIt, "fa-solid fa-headset", "",
                "https://servicedesk.hogent.be/tas/public/ssp/7d22f41a-42a7-49b0-89c4-ae1093aca8fd", false)
        };

        var meldingenEnAfwezigheid = new[]
        {
            new Shortcut("Afwezigheid lesgevers", ShortcutType.AlertsAndAbsence, "fa-solid fa-user-xmark", "",
                "https://chamilo.hogent.be/index.php?application=Chamilo\\Application\\Calendar", true),
            new Shortcut("Afwezigheid melden", ShortcutType.AlertsAndAbsence, "fa-solid fa-triangle-exclamation", "",
                "https://ibamaflex.hogent.be/", false),
        };

        var studentenlevenEnWelzijn = new[]
        {
            new Shortcut("Eerste hulp bij ongevallen", ShortcutType.StudentLifeAndWellbeing, "fa-solid fa-kit-medical",
                "", "https://hogent.sharepoint.com/sites/Preventieenmilieu-studenten/SitePages/Ongeval.aspx", false),
            new Shortcut("Vertrouwenspersoon", ShortcutType.StudentLifeAndWellbeing, "fa-solid fa-heart", "",
                "https://www.hogent.be/dit-is-hogent/beleid/grensoverschrijdend-gedrag/", false),
            new Shortcut("Ongeval melden", ShortcutType.StudentLifeAndWellbeing, "fa-solid fa-triangle-exclamation", "",
                "https://forms.office.com/Pages/ResponsePage.aspx?id=DjH3XBoJxUus1ybHIdTMzQnoEYG8HMRAgXs9u6ieGs5UODNZSzkyUzZOWE9HQ0lBQU1VNTRYVEJYRS4u",
                false),
            new Shortcut("Veiligheid / Gezondheid", ShortcutType.StudentLifeAndWellbeing, "fa-solid fa-shield-heart",
                "", "https://www.hogent.be/vgm/", false),
            new Shortcut("BIB HOGENT", ShortcutType.StudentLifeAndWellbeing, "fa-solid fa-book-open-reader", "",
                "https://www.hogent.be/student/bibliotheken/", false),
            new Shortcut("Campus Shop", ShortcutType.StudentLifeAndWellbeing, "fa-solid fa-cart-shopping", "",
                "https://leermaterialen.hogent.be/", false),
            new Shortcut("Mobiel printen", ShortcutType.StudentLifeAndWellbeing, "fa-solid fa-print", "",
                "https://mobielprinten.hogent.be/", false),
            new Shortcut("Studentenverenigingen", ShortcutType.StudentLifeAndWellbeing, "fa-solid fa-people-group", "",
                "studentenverenigingen", true),
            new Shortcut("Vacatures", ShortcutType.StudentLifeAndWellbeing, "fa-solid fa-briefcase", "", "vacatures",
                true),
            new Shortcut("Studentdeals", ShortcutType.StudentLifeAndWellbeing, "fa-solid fa-tag", "", "studenten-deals",
                true),
        };

        await dbContext.Shortcuts.AddRangeAsync(roostersEnKalenders);
        await dbContext.Shortcuts.AddRangeAsync(studiediensten);
        await dbContext.Shortcuts.AddRangeAsync(communicatieEnSoftware);
        await dbContext.Shortcuts.AddRangeAsync(ondersteuningEnIt);
        await dbContext.Shortcuts.AddRangeAsync(meldingenEnAfwezigheid);
        await dbContext.Shortcuts.AddRangeAsync(studentenlevenEnWelzijn);
    }

    private async Task AssignDefaultShortcutsToStudentAsync(Student student)
    {
        var defaultShortcuts = await dbContext.Shortcuts
            .Where(s => s.DefaultForGuest)
            .ToListAsync();

        var id = student.Id;
        var pos = 0;

        foreach (var s in defaultShortcuts)
        {
            UserShortcut us = new UserShortcut(id, s.Id, pos++);
            dbContext.UserShortcuts.Add(us);
        }
    }

    private async Task AssignDepartmentsAsync()
    {
        var departments = await dbContext.Departments.Include(department => department.Manager).ToListAsync();
        if (departments.Count == 0) return;

        var employees = await dbContext.Employees.ToListAsync();
        if (employees.Count == 0) return;

        foreach (var department in departments)
        {
            var manager = employees.FirstOrDefault(e => e.Department.Id == department.Id &&
                                                        (e.Title == "Departementshoofd" || e.Title == "Decaan" || e.Title.StartsWith("Directeur ") || e.Title.StartsWith("Diensthoofd ")));

            if (manager != null)
            {
                department.Manager ??= manager;
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private async Task MenuAsync()
    {
        if (dbContext.MenuItems.Any())
            return;

        var allMenuItems = new List<MenuItem>
        {
            new MenuItem
            {
                Name = "Spaghetti Bolognese", Description = "Lekkere spaghetti bolognese", IsVeganAndHalal = false,
                IsVeggieAndHalal = false, Category = FoodCategory.WarmeMaaltijd
            },
            new MenuItem
            {
                Name = "Margherita Pizza", Description = "Classic pizza met tomaat en mozzarella",
                IsVeganAndHalal = false, IsVeggieAndHalal = false, Category = FoodCategory.WarmeMaaltijd
            },
            new MenuItem
            {
                Name = "Pepperoni Pizza", Description = "Pizza met pepperoni en kaas", IsVeganAndHalal = false,
                IsVeggieAndHalal = false, Category = FoodCategory.WarmeMaaltijd
            },
            new MenuItem
            {
                Name = "Caesar Salad", Description = "Salade met kip, croutons en Parmezaanse kaas",
                IsVeganAndHalal = false, IsVeggieAndHalal = true, Category = FoodCategory.Groenten
            },
            new MenuItem
            {
                Name = "Falafel Wrap", Description = "Wrap met falafel, hummus en groenten", IsVeganAndHalal = true,
                IsVeggieAndHalal = false, Category = FoodCategory.Groenten
            },
            new MenuItem
            {
                Name = "Tomato Soup", Description = "Verse tomatensoep met kruiden", IsVeganAndHalal = true,
                IsVeggieAndHalal = false, Category = FoodCategory.Soep
            },
            new MenuItem
            {
                Name = "Grilled Chicken", Description = "Gegrilde kip met aardappelen en groenten",
                IsVeganAndHalal = false, IsVeggieAndHalal = false, Category = FoodCategory.WarmeMaaltijd
            },
            new MenuItem
            {
                Name = "Veggie Burger", Description = "Burger gemaakt van groenten en bonen", IsVeganAndHalal = true,
                IsVeggieAndHalal = true, Category = FoodCategory.Groenten
            },
            new MenuItem
            {
                Name = "Quinoa Bowl", Description = "Quinoa met geroosterde groenten en tahin dressing",
                IsVeganAndHalal = true, IsVeggieAndHalal = true, Category = FoodCategory.Groenten
            },
            new MenuItem
            {
                Name = "Fruit Salad", Description = "Vers gesneden fruit mix", IsVeganAndHalal = true,
                IsVeggieAndHalal = false, Category = FoodCategory.Wekelijks
            },
            new MenuItem
            {
                Name = "Cheesecake", Description = "Romige cheesecake met aardbeien", IsVeganAndHalal = false,
                IsVeggieAndHalal = false, Category = FoodCategory.Dessert
            },
            new MenuItem
            {
                Name = "Chocolade Muffin", Description = "Muffin met pure chocolade", IsVeganAndHalal = false,
                IsVeggieAndHalal = false, Category = FoodCategory.Dessert
            },
            new MenuItem
            {
                Name = "Lentil Stew", Description = "Hartige linzenschotel met groenten", IsVeganAndHalal = true,
                IsVeggieAndHalal = true, Category = FoodCategory.WarmeMaaltijd
            },
            new MenuItem
            {
                Name = "Sushi Bowl", Description = "Schaal met rijst, zalm, avocado en zeewier",
                IsVeganAndHalal = false, IsVeggieAndHalal = false, Category = FoodCategory.WarmeMaaltijd
            },
            new MenuItem
            {
                Name = "Pumpkin Soup", Description = "Romige pompoensoep met pompoenpitten", IsVeganAndHalal = true,
                IsVeggieAndHalal = false, Category = FoodCategory.Soep
            },
            new MenuItem
            {
                Name = "Greek Salad", Description = "Salade met feta, tomaat, komkommer en olijven",
                IsVeganAndHalal = false, IsVeggieAndHalal = true, Category = FoodCategory.Groenten
            },
            new MenuItem
            {
                Name = "Falafel Platter", Description = "Falafel met tabouleh en pittige saus", IsVeganAndHalal = true,
                IsVeggieAndHalal = false, Category = FoodCategory.Groenten
            },
            new MenuItem
            {
                Name = "Beef Stew", Description = "Stoofvlees met seizoensgroenten", IsVeganAndHalal = false,
                IsVeggieAndHalal = false, Category = FoodCategory.WarmeMaaltijd
            },
            new MenuItem
            {
                Name = "Pasta Pesto", Description = "Pasta met huisgemaakte pestosaus", IsVeganAndHalal = true,
                IsVeggieAndHalal = true, Category = FoodCategory.WarmeMaaltijd
            },
            new MenuItem
            {
                Name = "Apple Crumble", Description = "Warme appelcrumble met vanillesaus", IsVeganAndHalal = false,
                IsVeggieAndHalal = false, Category = FoodCategory.Dessert
            },
            new MenuItem
            {
                Name = "Miso Soup", Description = "Warme miso-soep met zeewier en tofu", IsVeganAndHalal = true,
                IsVeggieAndHalal = true, Category = FoodCategory.Soep
            },
            new MenuItem
            {
                Name = "Yoghurt Parfait", Description = "Yoghurt met granola en vers fruit", IsVeganAndHalal = false,
                IsVeggieAndHalal = false, Category = FoodCategory.Wekelijks
            },
            new MenuItem
            {
                Name = "Wholegrain Sandwich", Description = "Volkoren sandwich met hummus en groenten",
                IsVeganAndHalal = true, IsVeggieAndHalal = true, Category = FoodCategory.Wekelijks
            },
            new MenuItem
            {
                Name = "Frieten", Description = "Belgische frieten met mayonaise", IsVeganAndHalal = true,
                IsVeggieAndHalal = true, Category = FoodCategory.Zetmeel
            },
            new MenuItem
            {
                Name = "Gekookte Aardappelen", Description = "Gekookte aardappelen met kruidenboter",
                IsVeganAndHalal = false, IsVeggieAndHalal = true, Category = FoodCategory.Zetmeel
            },
            new MenuItem
            {
                Name = "Witte Rijst", Description = "Stoomrijst (basmati)", IsVeganAndHalal = true,
                IsVeggieAndHalal = true, Category = FoodCategory.Zetmeel
            },
            new MenuItem
            {
                Name = "Nacho's met salsa", Description = "Knapperige nacho's met pittige salsa",
                IsVeganAndHalal = true, IsVeggieAndHalal = true, Category = FoodCategory.Snacks
            },
            new MenuItem
            {
                Name = "Kipnuggets", Description = "Goudbruin gefrituurde kipnuggets", IsVeganAndHalal = false,
                IsVeggieAndHalal = false, Category = FoodCategory.Snacks
            },
            new MenuItem
            {
                Name = "Hummus met pita", Description = "Romige hummus met warme pita", IsVeganAndHalal = true,
                IsVeggieAndHalal = true, Category = FoodCategory.Snacks
            },
            new MenuItem
            {
                Name = "Broodje Ham & Kaas", Description = "Vers broodje met ham en kaas", IsVeganAndHalal = false,
                IsVeggieAndHalal = false, Category = FoodCategory.Broodjes
            },
            new MenuItem
            {
                Name = "Broodje Kip Caesar", Description = "Broodje met gegrilde kip en caesardressing",
                IsVeganAndHalal = false, IsVeggieAndHalal = false, Category = FoodCategory.Broodjes
            },
            new MenuItem
            {
                Name = "Broodje Hummus & Groenten", Description = "Volkoren broodje met hummus en geroosterde groenten",
                IsVeganAndHalal = true, IsVeggieAndHalal = true, Category = FoodCategory.Broodjes
            }
        };

        await dbContext.MenuItems.AddRangeAsync(allMenuItems);
        await dbContext.SaveChangesAsync();

        var menu1 = new List<MenuItem>
        {
            allMenuItems[0], allMenuItems[5], allMenuItems[9], allMenuItems[10], allMenuItems[1], allMenuItems[14],
            allMenuItems[23], allMenuItems[26]
        };
        var menu2 = new List<MenuItem>
        {
            allMenuItems[2], allMenuItems[3], allMenuItems[14], allMenuItems[11], allMenuItems[4], allMenuItems[15],
            allMenuItems[21], allMenuItems[29]
        };
        var menu3 = new List<MenuItem>
        {
            allMenuItems[6], allMenuItems[7], allMenuItems[12], allMenuItems[19], allMenuItems[8], allMenuItems[13],
            allMenuItems[24], allMenuItems[10]
        };
        var menu4 = new List<MenuItem>
        {
            allMenuItems[1], allMenuItems[8], allMenuItems[15], allMenuItems[18], allMenuItems[3], allMenuItems[16],
            allMenuItems[25], allMenuItems[27]
        };
        var menu5 = new List<MenuItem>
        {
            allMenuItems[13], allMenuItems[4], allMenuItems[16], allMenuItems[17], allMenuItems[9], allMenuItems[2],
            allMenuItems[30], allMenuItems[28]
        };

        var menuForWeek = new Menu
        {
            StartDate = DateTime.Now,
            HasMenu = true
        };
        var menuForWeek2 = new Menu
        {
            StartDate = DateTime.Now,
            HasMenu = false,
            DescriptionMenu =
                "In gebouw B huist voortaan de Picadeli salad bar, waar je zelf een slaatje kan samenstellen naar eigen smaak en goesting. Je betaalt per gewicht: 1,30 euro per 100 gram (studentenprijs). Je kan ter plaatse eten of je slaatje meenemen. Soep en broodjes aan studentenprijzen blijven ook deel uitmaken van het gamma."
        };

        menuForWeek.AddMenuToDay("Ma", menu1);
        menuForWeek.AddMenuToDay("Di", menu2);
        menuForWeek.AddMenuToDay("Wo", menu3);
        menuForWeek.AddMenuToDay("Do", menu4);
        menuForWeek.AddMenuToDay("Vr", menu5);


        await dbContext.Menus.AddRangeAsync(menuForWeek, menuForWeek2);
        var restos = await dbContext.Restos.Include(resto => resto.Building).ThenInclude(building => building!.Campus)
            .ToListAsync();
        var restoBijloke = restos.First(r => r.Building != null && r.Building.Campus.Name.Equals("Campus Bijloke"));
        var restomercator = restos.Where(r => r.Building != null && r.Building.Campus.Name.Equals("Campus Mercator"))
            .First();
        var restoLedeganck = restos.Where(r => r.Building != null && r.Building.Campus.Name.Equals("Campus Ledeganck"))
            .First();
        var restoSchoonmeersenB = restos.Where(b =>
            b.Building != null && b.Building.Name.Equals("Gebouw B") &&
            b.Building.Campus.Name.Equals("Campus Schoonmeersen")).First();
        var restoSchoonmeersenP = restos.Where(b =>
            b.Building != null && b.Building.Name.Equals("Gebouw P") &&
            b.Building.Campus.Name.Equals("Campus Schoonmeersen")).First();
        var restoSchoonmeersenD = restos.Where(b =>
            b.Building != null && b.Building.Name.Equals("Gebouw D") &&
            b.Building.Campus.Name.Equals("Campus Schoonmeersen")).First();
        restoBijloke.Menu = menuForWeek;
        restomercator.Menu = menuForWeek;
        restoSchoonmeersenB.Menu = menuForWeek2;
        restoSchoonmeersenP.Menu = menuForWeek;
        restoSchoonmeersenD.Menu = menuForWeek;
        restoLedeganck.Menu = menuForWeek;
        dbContext.Restos.UpdateRange(restoBijloke, restomercator, restoSchoonmeersenB, restoSchoonmeersenP,
            restoSchoonmeersenD, restoLedeganck);

        await dbContext.SaveChangesAsync();
    }

    private async Task InfrastructureAsync()
    {
        if (dbContext.Campuses.Any()) return;


        // Seed campuses
        // =============

        var schoonmeersenCampus = new Campus
        {
            Name = "Campus Schoonmeersen",
            Description =
                "Campus Schoonmeersen is vlot bereikbaar en goed gelegen aan de ring rond Gent (R4) en op wandelafstand van station Gent Sint-Pieters (600 m). De bus- en tramhalte ‘Gent Tuinwijklaan’ heeft een vlotte verbinding met het centrum en de randgemeentes. Gebouw T bevindt zich een halte verder, aan bus- en tramhalte ‘Gent Flamingostraat’.\n\nAls HOGENT-student of CVO-cursist kan je op basis van je studentennummer een parkeervignet aanvragen. Met een geldig parkeervignet kan je tijdens de lessen gratis parkeren op de centrale bovengrondse parking op campus Schoonmeersen. Kijk op hogent.be/parkeren voor meer informatie en het parkeerreglement.\n\nAndere bezoekers kunnen terecht in betaalparking Gent Sint-Pieters. Kijk op belgiantrain.be voor meer informatie over parkeertarieven en parkeerduur.\n\nVanaf de gratis Park & Ride The Loop/Expo is het 9 minuten fietsen tot campus Schoonmeersen (2,4 km). Kijk op stad.gent/parkeren voor meer informatie over deze Park & Ride en over de beschikbare deelfietsen ter plekke.\n\nCampus Schoonmeersen ligt buiten de lage-emissiezone (LEZ). Kijk op stad.gent/lez voor meer informatie over de lage-emissiezone in Gent.",
            Address = new Address("Valentin Vaerwyckweg", "1", "9000", "Gent"),
            ImageUrl = "Schoonmeersen.jpg",
            TourUrl = "https://youreka-virtualtours.be/tours/hogent_schoonmeersen/",
            MapsUrl =
                "https://www.google.com/maps/dir/?api=1&destination=HOGENT+campus+Schoonmeersen+Valentin+Vaerwyckweg+1+Gent&travelmode=transit",
            Facilities = new CampusFacilities(true, true, true, true, true, true, true, true, true, true, true),
            Buildings = new List<Building>(),
        };

        var mercatorCampus = new Campus
        {
            Name = "Campus Mercator",
            Description =
                "Campus Mercator bevindt zich ideaal gelegen op 4 minuutjes fietsen of 12 minuten wandelen van het centrum en van station Gent Sint-Pieters (1 km). Je kan ook de tram nemen tot aan de halte ‘Gent Koning Albertbrug’ (200 m) of halte ‘Gent Van Nassaustraat’ (450 m).\n\nIn de directe omgeving van campus Mercator kan je betalend parkeren op straat (groene zone). Kijk op stad.gent/parkeren voor meer informatie over parkeertarieven en parkeerduur.\n\nCampus Mercator ligt buiten de lage-emissiezone (LEZ). Kijk op stad.gent/lez voor meer informatie over de lage-emissiezone in Gent.",
            Address = new Address("Henleykaai", "84", "9000", "Gent"),
            ImageUrl = "Mercator.jpg",
            TourUrl = "https://youreka-virtualtours.be/tours/hogent_mercator/",
            MapsUrl =
                "https://www.google.com/maps/dir/?api=1&destination=HOGENT+campus+Mercator+Henleykaai+84+Gent&travelmode=transit",
            Facilities = new CampusFacilities(true, true, true, true, true, true, true, true, true, true, true),
            Buildings = new List<Building>(),
        };

        var ledeganckCampus = new Campus
        {
            Name = "Campus Ledeganck",
            Description =
                "Campus Ledeganck ligt op 5 minuutjes fietsen of 15 minuten wandelen van het station Gent Sint-Pieters, aan de andere kant van het Citadelpark (1 km), vlak aan de kleine ring van Gent (R40) en de Overpoort.\n\nIn de Ledeganckstraat kan je betalend parkeren op straat (groene zone). Kijk op stad.gent/parkeren voor meer informatie over parkeertarieven en parkeerduur.",
            Address = new Address("K.L. Ledeganckstraat", "35", "9000", "Gent"),
            ImageUrl = "Ledeganck.jpg",
            TourUrl = "https://youreka-virtualtours.be/tours/hogent_ledeganck/",
            MapsUrl =
                "https://www.google.com/maps/dir/?api=1&destination=HOGENT+campus+Ledeganck+Karel+Lodewijk+Ledeganckstraat+35+Gent&travelmode=transit",
            Facilities = new CampusFacilities(true, true, true, true, true, true, true, true, true, true, true),
            Buildings = new List<Building>(),
        };

        var bijlokeCampus = new Campus
        {
            Name = "Campus Bijloke",
            Description =
                "Campus Bijloke bevindt zich ideaal gelegen aan de kleine ring van Gent (R40), op 6 minuutjes fietsen of 15 minuten wandelen van het station Gent Sint-Pieters (1,5 km). Je kan ook de tram nemen tot aan de halte ‘Gent Bijlokehof’.\n\nIn de directe omgeving van campus Bijloke kan je betalend parkeren op straat (oranje zone). Aan de overkant van de R40 start de groene zone en heb je een gunstiger parkeertarief. Kijk op stad.gent/parkeren voor meer informatie over de parkeertarieven, zones en parkeerduur.\n\nCampus Bijloke ligt binnen de lage-emissiezone (LEZ). Kijk op stad.gent/lez voor meer informatie over de lage-emissiezone in Gent en om te testen of je voertuig binnen mag in deze zone.",
            Address = new Address("Louis Pasteurlaan", "2", "9000", "Gent"),
            ImageUrl = "Bijloke.jpg",
            MapsUrl =
                "https://www.google.com/maps/dir/?api=1&destination=HOGENT+campus+Bijloke+Louis+Pasteurlaan+2+Gent&travelmode=transit",
            Facilities = new CampusFacilities(true, true, true, true, true, true, true, true, true, true, true),
            Buildings = new List<Building>(),
        };

        var aalstCampus = new Campus
        {
            Name = "Campus Aalst",
            Description =
                "Campus Aalst is een gezellige campus in het centrum van de stad, op wandelafstand van station Aalst (850 m), parking Keizershallen (350 m) en parking Hopmarkt (350 m). Een dynamisch ecosysteem dankzij de nauwe band tussen lesgevers, studenten en de Aalsterse ondernemers, bedrijven en organisaties.",
            Address = new Address("Arbeidstraat", "14", "9300", "Aalst"),
            ImageUrl = "Aalst.jpg",
            MapsUrl =
                "https://www.google.com/maps/dir/?api=1&destination=HOGENT+campus+Aalst+Arbeidstraat+14+Aalst&travelmode=transit",
            Facilities = new CampusFacilities(true, true, true, true, true, true, true, true, true, true, true),
            Buildings = new List<Building>(),
        };

        await dbContext.Campuses.AddRangeAsync(schoonmeersenCampus, mercatorCampus, bijlokeCampus, ledeganckCampus,
            aalstCampus);
        await dbContext.SaveChangesAsync();


        // Seed buildings
        // ==============

        var schoonmeersenBuildingB = new Building
        {
            Name = "Gebouw B",
            Description =
                "Gebouw B is het oorspronkelijke theorie‑ en onderwijsgebouw op Campus Schoonmeersen. Het gebouw huisvest vooral grote leslokalen, auditoria en studentenzones zoals een cafetaria en onthaal. Vanaf hier kunnen studenten makkelijk wandelen naar de andere gebouwen op de campus, en je vindt er ook het onthaal voor bezoekers.",
            Address = new Address(schoonmeersenCampus.Address.Addressline1, schoonmeersenCampus.Address.Addressline2,
                schoonmeersenCampus.Address.City, schoonmeersenCampus.Address.PostalCode),
            ImageUrl = "GebouwB.jpg",
            Campus = schoonmeersenCampus,
            Classrooms = new List<Classroom>(),
            Restos = new List<Resto>(),
            Facilities = new CampusFacilities(false, false, false, false, true, false, true, false, false, true, true)
        };

        var schoonmeersenBuildingP = new Building
        {
            Name = "Gebouw P",
            Description =
                "Gebouw P ligt centraal op de campus en vormt een belangrijk ontmoetingspunt. Het huisvest het studentenrestaurant, grote aula’s en kleinere lezingenruimtes. Het is een plek waar studenten elkaar vaak ontmoeten tussen de lessen door.",
            Address = new Address(schoonmeersenCampus.Address.Addressline1, schoonmeersenCampus.Address.Addressline2,
                schoonmeersenCampus.Address.City, schoonmeersenCampus.Address.PostalCode),
            ImageUrl = "GebouwP.jpg",
            Campus = schoonmeersenCampus,
            Classrooms = new List<Classroom>(),
            Restos = new List<Resto>(),
            Facilities = new CampusFacilities(false, false, false, false, true, false, true, true, false, false, true)
        };

        var schoonmeersenBuildingD = new Building
        {
            Name = "Gebouw D",
            Description =
                "Gebouw D is een typisch lesgebouw met meerdere klaslokalen, projectruimtes en kleinere practica. Het is ideaal voor groepswerk, actieve opdrachten en teamprojecten en wordt veel gebruikt door studentengroepen.",
            Address = new Address(schoonmeersenCampus.Address.Addressline1, schoonmeersenCampus.Address.Addressline2,
                schoonmeersenCampus.Address.City, schoonmeersenCampus.Address.PostalCode),
            ImageUrl = "GebouwD.jpg",
            Campus = schoonmeersenCampus,
            Classrooms = new List<Classroom>(),
            Restos = new List<Resto>(),
            Facilities = new CampusFacilities(true, true, true, false, true, true, true, true, false, false, false)
        };

        var schoonmeersenBuildingT = new Building
        {
            Name = "Gebouw T",
            Description =
                "Gebouw T is een modern les‑ en hoorcollegegebouw op Campus Schoonmeersen. Het bevat tien auditoria en een twintigtal kleinere lokalen rond een groot atrium. De infrastructuur is ontworpen voor hogeschoolonderwijs met focus op bereikbaarheid, licht en samenwerking.",
            Address = new Address("Voskenslaan", "364A", "9000", "Gent"),
            ImageUrl = "GebouwT.jpg",
            Campus = schoonmeersenCampus,
            Classrooms = new List<Classroom>(),
            Restos = new List<Resto>(),
            Facilities = new CampusFacilities(false, false, false, false, true, false, true, true, false, false, true)
        };

        var schoonmeersenBuildingC = new Building
        {
            Name = "Gebouw C",
            Description =
                "Gebouw C is een onderwijsgebouw met klaslokalen, projectruimtes en ondersteunende infrastructuur. Het bevat geen sporthal, maar biedt kwalitatieve leeromgevingen voor verschillende studierichtingen.",
            Address = new Address(schoonmeersenCampus.Address.Addressline1, schoonmeersenCampus.Address.Addressline2,
                schoonmeersenCampus.Address.City, schoonmeersenCampus.Address.PostalCode),
            ImageUrl = "GebouwC.jpg",
            Campus = schoonmeersenCampus,
            Classrooms = new List<Classroom>(),
            Restos = new List<Resto>(),
            Facilities = new CampusFacilities(true, true, true, false, true, true, true, true, false, false, false)
        };

        var schoonmeersenBuildingE = new Building
        {
            Name = "Gebouw E",
            Description =
                "Gebouw E heeft een ondersteunende of polyvalente functie en huisvest kantoren, leslokalen of voorzieningen die minder prominent door studenten worden gebruikt.",
            Address = new Address(schoonmeersenCampus.Address.Addressline1, schoonmeersenCampus.Address.Addressline2,
                schoonmeersenCampus.Address.City, schoonmeersenCampus.Address.PostalCode),
            ImageUrl = "GebouwE.png",
            Campus = schoonmeersenCampus,
            Classrooms = new List<Classroom>(),
            Restos = new List<Resto>(),
            Facilities = new CampusFacilities(false, false, false, false, false, false, false, false, false, false,
                false)
        };

        var schoonmeersenBuildingS = new Building
        {
            Name = "Sporthal",
            Description =
                "De sporthal op Campus Schoonmeersen is een moderne sportinfrastructuur met meerdere polyvalente hallen, turn‑ en danszalen, kleedkamers en douches. De hal is geschikt voor volleybal, basketbal, zaalvoetbal, badminton en andere sporten en is toegankelijk voor studenten, personeel en externe gebruikers.",
            Address = new Address(schoonmeersenCampus.Address.Addressline1, schoonmeersenCampus.Address.Addressline2,
                schoonmeersenCampus.Address.City, schoonmeersenCampus.Address.PostalCode),
            ImageUrl = "GebouwS.jpg",
            Campus = schoonmeersenCampus,
            Classrooms = new List<Classroom>(),
            Restos = new List<Resto>(),
            Facilities = new CampusFacilities(false, false, false, false, true, false, false, true, true, false, true)
        };

        schoonmeersenCampus.AddBuilding(schoonmeersenBuildingB);
        schoonmeersenCampus.AddBuilding(schoonmeersenBuildingP);
        schoonmeersenCampus.AddBuilding(schoonmeersenBuildingD);
        schoonmeersenCampus.AddBuilding(schoonmeersenBuildingT);
        schoonmeersenCampus.AddBuilding(schoonmeersenBuildingC);
        schoonmeersenCampus.AddBuilding(schoonmeersenBuildingE);
        schoonmeersenCampus.AddBuilding(schoonmeersenBuildingS);

        var mercatorBuildingG = new Building
        {
            Name = "Gebouw G",
            Description = "Hoofdgebouw op Mercator.",
            Address = new Address(mercatorCampus.Address.Addressline1, mercatorCampus.Address.Addressline2,
                mercatorCampus.Address.City, mercatorCampus.Address.PostalCode),
            ImageUrl = "https://example.com/gebouw-g.jpg",
            Campus = mercatorCampus,
            Classrooms = new List<Classroom>(),
            Restos = new List<Resto>(),
            Facilities = new CampusFacilities(false, false, false, false, true, false, true, false, false, true, true)
        };

        mercatorCampus.AddBuilding(mercatorBuildingG);

        var bijlokeBuildingB = new Building
        {
            Name = "Gebouw B",
            Description = "Kunstgebouw op Bijloke.",
            Address = new Address(bijlokeCampus.Address.Addressline1, bijlokeCampus.Address.Addressline2,
                bijlokeCampus.Address.City, bijlokeCampus.Address.PostalCode),
            ImageUrl = "https://example.com/gebouw-b-bijloke.jpg",
            Campus = bijlokeCampus,
            Classrooms = new List<Classroom>(),
            Restos = new List<Resto>(),
            Facilities = new CampusFacilities(false, false, false, false, true, false, true, false, false, true, true)
        };

        bijlokeCampus.AddBuilding(bijlokeBuildingB);

        var ledeganckBuildingA = new Building
        {
            Name = "Gebouw A",
            Description = "Wetenschapsgebouw op Ledeganck.",
            Address = new Address(ledeganckCampus.Address.Addressline1, ledeganckCampus.Address.Addressline2,
                ledeganckCampus.Address.City, ledeganckCampus.Address.PostalCode),
            ImageUrl = "https://example.com/gebouw-a.jpg",
            Campus = ledeganckCampus,
            Classrooms = new List<Classroom>(),
            Restos = new List<Resto>(),
            Facilities = new CampusFacilities(false, false, false, false, true, false, true, false, false, true, true)
        };

        ledeganckCampus.AddBuilding(ledeganckBuildingA);

        await dbContext.Buildings.AddRangeAsync(
            schoonmeersenBuildingB, schoonmeersenBuildingP, schoonmeersenBuildingD, schoonmeersenBuildingC,
            schoonmeersenBuildingT, schoonmeersenBuildingE, schoonmeersenBuildingS,
            mercatorBuildingG, bijlokeBuildingB, ledeganckBuildingA);
        await dbContext.SaveChangesAsync();


        // Seed classrooms
        // ===============
        var mockRooms = new List<(string Number, string Name, string Description, string Category, string Floor)>
        {
            ("010", "01.02.100.010", "GSCHB.0.010 BCON", "auditorium", "0"),
            ("064", "01.02.100.064", "GSCHB.0.064", "auditorium", "0"),
            ("007", "01.02.110.007", "GSCHB.1.007", "laptoplokaal", "1"),
            ("008", "01.02.110.008", "GSCHB.1.008", "leslokaal", "1"),
            ("011", "01.02.110.011", "GSCHB.1.011", "laptoplokaal", "1"),
            ("012", "01.02.110.012", "GSCHB.1.012", "leslokaal", "1"),
            ("014", "01.02.110.014", "GSCHB.1.014", "laptoplokaal", "1"),
            ("015", "01.02.110.015", "GSCHB.1.015", "auditorium", "1"),
            ("017", "01.02.110.017", "GSCHB.1.017", "auditorium", "1"),
            ("026", "01.02.110.026", "GSCHB.1.026", "laptoplokaal", "1"),
            ("029", "01.02.110.029", "GSCHB.1.029", "laptoplokaal", "1"),
            ("031", "01.02.110.031", "GSCHB.1.031 Laptoplokaal", "laptoplokaal", "1"),
            ("032", "01.02.110.032", "GSCHB.1.032 Auditorium", "auditorium", "1"),
            ("034", "01.02.110.034", "GSCHB.1.034", "auditorium", "1"),
            ("036", "01.02.110.036", "GSCHB.1.036", "laptoplokaal", "1"),
            ("037", "01.02.110.037", "GSCHB.1.037", "laptoplokaal", "1"),
            ("038", "01.02.110.038", "GSCHB.1.038 Labo fysica", "laboratorium", "1"),
            ("005", "01.02.120.005", "GSCHB.2.005", "leslokaal", "2"),
            ("006", "01.02.120.006", "GSCHB.2.006", "leslokaal", "2"),
            ("007", "01.02.120.007", "GSCHB.2.007", "leslokaal", "2")
        };

        foreach (var room in mockRooms)
        {
            var classroom = new Classroom
            {
                Number = room.Number,
                Name = room.Name,
                Description = room.Description,
                Category = room.Category,
                Floor = room.Floor,
                Building = schoonmeersenBuildingB
            };
            schoonmeersenBuildingB.AddClassroom(classroom);
            dbContext.Classrooms.Add(classroom);
        }

        var mockClassroomP = new Classroom
        {
            Number = "001",
            Name = "P.001",
            Description = "Lab in Gebouw P",
            Category = "laboratorium",
            Floor = "0",
            Building = schoonmeersenBuildingP
        };
        schoonmeersenBuildingP.AddClassroom(mockClassroomP);

        var mockClassroomD = new Classroom
        {
            Number = "101",
            Name = "D.101",
            Description = "Auditorium in Gebouw D",
            Category = "auditorium",
            Floor = "1",
            Building = schoonmeersenBuildingD
        };
        schoonmeersenBuildingD.AddClassroom(mockClassroomD);

        var mockClassroomG = new Classroom
        {
            Number = "201",
            Name = "G.201",
            Description = "Leslokaal in Gebouw G",
            Category = "leslokaal",
            Floor = "2",
            Building = mercatorBuildingG
        };
        mercatorBuildingG.AddClassroom(mockClassroomG);

        var mockClassroomBijloke = new Classroom
        {
            Number = "B001",
            Name = "Bijloke B.001",
            Description = "Studio in Gebouw B",
            Category = "studio",
            Floor = "0",
            Building = bijlokeBuildingB
        };
        bijlokeBuildingB.AddClassroom(mockClassroomBijloke);

        var mockClassroomLedeganck = new Classroom
        {
            Number = "A101",
            Name = "Ledeganck A.101",
            Description = "Lab in Gebouw A",
            Category = "laboratorium",
            Floor = "1",
            Building = ledeganckBuildingA
        };
        ledeganckBuildingA.AddClassroom(mockClassroomLedeganck);

        await dbContext.SaveChangesAsync();


        // Seed Restos
        // ===========
        var restoNames = new List<string>
        {
            "Ledeganck",
            "Mercator G.",
            "Bijloke",
            "Schoonmeersen B.",
            "Schoonmeersen P.",
            "Schoonmeersen D."
        };

        var restos = new List<Resto>();
        for (int i = 0; i < restoNames.Count; i++)
        {
            var resto = new Resto
            {
                Name = $"Resto {restoNames[i]}",
                Coordinates = $"{i}, {i + 1}",
            };

            switch (i)
            {
                case 0:
                    resto.Building = ledeganckBuildingA;
                    ledeganckBuildingA.AddResto(resto);
                    break;
                case 1:
                    resto.Building = mercatorBuildingG;
                    mercatorBuildingG.AddResto(resto);
                    break;
                case 2:
                    resto.Building = bijlokeBuildingB;
                    bijlokeBuildingB.AddResto(resto);
                    break;
                case 3:
                    resto.Building = schoonmeersenBuildingB;
                    schoonmeersenBuildingB.AddResto(resto);
                    break;
                case 4:
                    resto.Building = schoonmeersenBuildingP;
                    schoonmeersenBuildingP.AddResto(resto);
                    break;
                case 5:
                    resto.Building = schoonmeersenBuildingD;
                    schoonmeersenBuildingD.AddResto(resto);
                    break;
            }

            restos.Add(resto);
        }

        await dbContext.Restos.AddRangeAsync(restos);
        await dbContext.SaveChangesAsync();
    }

    private async Task EventsAsync()
    {
        if (dbContext.Events.Any()) return;

        var now = DateTime.UtcNow;
        var random = new Random();

        var eventList = new List<Event>
        {
            new()
            {
                Title = "Eéndaagse opleidingen fondsenwervingsmix",
                Date = new EventTimeSlot(
                    DateOnly.FromDateTime(now.AddDays(30 + random.Next(0, 30))),
                    new TimeOnly(14, 0), new TimeOnly(16, 30)),
                ImageUrl = "event4.png",
                Subject = Topics.Education.Name,
                Content = @"
                            <div class=""fondsenwerving-training"">
                              <h3>
                                Ben je in een non-profitorganisatie actief rond o.a. giften, sponsoring en legaten?
                                Verdiep je in de fondsenwervingsmix en leer via verschillende experten en inspirerende voorbeelden
                                uit de sector in één dag hoe je een bepaalde fondsenwervingstechniek toepast om je doelen te bereiken.
                              </h3>

                              <p>
                                Deze ééndaagse trainingen worden georganiseerd door HOGENT en Fundraisers Belgium.
                                De opleidingen zijn van het type ‘how to start’: na één dag weet je of de techniek geschikt is voor jouw organisatie,
                                hoe je ermee aan de slag kunt, en hoe je die (verder) professioneel kunt implementeren.
                                De theorie wordt aangevuld met best practices die inspireren.
                              </p><br />

                              <h4><b>Troeven</b></h4><br />
                              <ul>
                                <li>
                                  Je leert snel nieuwe vaardigheden bij. Zonder een langdurig studie-engagement aan te gaan,
                                  beheers je in weinig tijd specifieke kennis en dit voor een relatief laag inschrijvingsgeld.
                                </li>
                                <li>
                                  Je kan zelf je keuzepakket samenstellen en afstemmen op de expertisedomeinen van jou en de organisatie(s)
                                  waarvoor je werkt. Door de modulaire opbouw per specifieke expertise krijg je toegang tot nieuwe inzichten,
                                  technieken en ontwikkelingen waar je direct mee aan de slag kan binnen jouw organisatie.
                                </li>
                                <li>Je krijgt toegang tot een waardevol netwerk.</li>
                                <li>Voor elke bijkomende sessie die je bijboekt krijg je 20% korting.</li>
                                <li>
                                  Voor elke bijkomende persoon uit dezelfde organisatie die je bijboekt
                                  krijg je 20% korting en betaal je dus 360 euro voor de tweede, derde, ...
                                  inschrijving.
                                </li>
                              </ul>
                            </div>
                            ",
                Address = new Address("Stadsparklaan 1", "", "Gent", "9000"),
                RegisterLink = "https://www.google.com/",
                Price = 4.50
            },

            new()
            {
                Title = "Gentle Teaching bij hoogspanning",
                Date = new EventTimeSlot(
                    DateOnly.FromDateTime(now.AddDays(45 + random.Next(0, 30))),
                    new TimeOnly(14, 0)),
                ImageUrl = "event3.png",
                Subject = Topics.Education.Name,
                Content = @"
                            <div class=""gentle-teaching-studiedag"">
                              <h3>
                                Gentle Teaching plaatst de relatie tussen cliënt en hulpverlener centraal.
                                Het vraagt van hulpverleners een onvoorwaardelijke houding, het bieden van veiligheid
                                en het vermijden van dwang en macht. Maar hoe doe je dat in omstandigheden die allesbehalve ideaal zijn?
                                In deze studiedag proberen we antwoord te bieden op deze vraag!
                              </h3>

                              <p>Je krijgt antwoorden op vragen als:</p>
                              <ul>
                                <li>Wat als hulpverleners onder druk staan en houvast zoeken in controle en macht?</li>
                                <li>Wat als angst of onzekerheid de bovenhand neemt?</li>
                                <li>Wat als verbinden niet (meer) lijkt te lukken?</li>
                              </ul><br />

                              <p>
                                Er is ook bespreking en uitwisseling van factoren binnen de context – cliënt en hulpverlener die druk kunnen leggen op de begeleidersrelatie.
                                Een gevarieerd programma zal je inspireren en ondersteunen. Ontdek samen met ons hoe waardevol Gentle Teaching is,
                                zeker wanneer je in troebel water vaart!
                              </p><br />

                              <h4><b>Programma</b></h4>

                              <div class=""programma-schema"">
                                <div class=""blok"">
                                  <div class=""tijd"">9.00 uur</div>
                                  <div class=""inhoud"">
                                    <span>Onthaal en welkom</span><br />
                                    <span>koffie-ontvangst</span>
                                  </div>
                                </div><br />

                                <div class=""blok"">
                                  <div class=""tijd"">09.30 uur</div>
                                  <div class=""inhoud"">
                                    <span>Plenaire lezingen</span>
                                    <ul>
                                      <li>Hoe meer hoogspanning, hoe krachtiger Gentle Teaching.</li>
                                      <li>Wie het luidst roept heeft het kleinste hartje.</li>
                                      <li>Kortsluiting of krachtveld. Het potentieel in de spanning tussen zorg en tijdsgeest.</li>
                                    </ul>
                                  </div>
                                </div><br />

                                <div class=""blok"">
                                  <div class=""tijd"">12.30 uur</div>
                                  <div class=""inhoud"">
                                    <span>Slotbespreking plenaire lezingen</span><br />
                                    <span>Middagpauze</span><br />
                                    <span>incl. broodjeslunch</span>
                                  </div>
                                </div><br />

                                <div class=""blok"">
                                  <div class=""tijd"">13.30 uur</div>
                                  <div class=""inhoud"">
                                    <span>Keuzeworkshops</span>
                                    <p>
                                      Kies zelf twee namiddagworkshops in functie van je interesses.
                                      De meeste workshops lopen van 13.30 tot 14.45 uur en van 15.15 tot 16.30 uur.
                                    </p>
                                    <ul>
                                      <li>Wederzijdse emotionele beschikbaarheid – Mentemo</li>
                                      <li>Trauma en verstandelijke beperking (impact op brein, lichaam en emotionele ontwikkeling) – Rudi De Rugzak</li>
                                      <li>Als relatie-opbouw niet van een leien dakje loopt of onder druk staat: perspectieven vanuit Emotionele Ontwikkeling en Gentle Teaching</li>
                                      <li>Gentle omgaan met de dwangmatige buitenkant van een cliënt</li>
                                      <li>Renoveren (onder stroom: verbinding herstellen tussen mens en maatschappij) via kwartiermakend hulpverlenen</li>
                                      <li>Hoe trekken we het schip weer vlot?</li>
                                      <li>De kracht van het verhaal</li>
                                    </ul>
                                  </div>
                                </div><br />

                                <div class=""blok"">
                                  <div class=""tijd"">16.30 uur</div>
                                  <div class=""inhoud"">
                                    <span>Netwerkmoment</span>
                                  </div>
                                </div><br />

                                <div class=""blok"">
                                  <div class=""tijd"">18.00 uur</div>
                                  <div class=""inhoud"">
                                    <span>Einde</span>
                                  </div>
                                </div>
                              </div>
                            </div>
                            ",
                Address = new Address("Stadsparklaan 1", "", "Gent", "9000"),
                Price = 6.00,
                RegisterLink = "https://www.eventbrite.be/e/tickets-gentle-teaching-1743352477189?aff=oddtdtcreator"
            },

            new()
            {
                Title = "Een diepgaandere blik op eetstoornissen.",
                Date = new EventTimeSlot(
                    DateOnly.FromDateTime(now.AddDays(60 + random.Next(0, 30))),
                    new TimeOnly(14, 0), new TimeOnly(16, 30)),
                ImageUrl = "event1.png",
                Subject = Topics.Education.Name,
                Content = @"
                            <div class=""eetstoornissen-opleiding"">
                              <h3>
                                Eetstoornissen gaan gepaard met heel wat fysieke én mentale hindernissen.
                                Ook als hulpverlener kom je voor tal van uitdagingen te staan.
                                Deze reeks bijscholingen wil jou hierbij ondersteunen.
                                De meest actuele thema's komen aan bod in 4 sessies waarbij we vertrekken van een holistische blik
                                en een multidisciplinaire benadering. Kies en/of mix de sessies die je interesseren, of volg ze ze allemaal.
                              </h3>

                              <p>
                                Je verdiept je kennis en expertise met recente onderzoeksresultaten, maar evenzeer keer je huiswaarts met
                                een combinatie van wetenschappelijke onderbouwing én praktijkhandvatten die je snel en probleemloos kan
                                implementeren in je eigen werkomgeving. Je volgt deze sessies samen met de studenten van het postgraduaat
                                eetstoornissen. Experts, onderzoekers en ervaringsdeskundigen delen hun inzichten, zodat jij vlot de vertaalslag
                                maakt van de opgedane informatie naar de doelgroep van jouw professionele praktijk.
                              </p><br />

                              <h4><b>Programma 2025–26</b></h4>
                              <p>
                                De sessies vinden telkens plaats op dinsdag van 9 tot 17 uur.
                                Noteer alvast in je agenda onderstaande data en onderwerpen. Meer info volgt later.
                              </p><br />

                              <div class=""programma-sessies"">
                                <div class=""sessie"">
                                  <div class=""datum"">
                                    <span class=""dag"">2</span>
                                    <span class=""maand"">december</span>
                                  </div>
                                  <div class=""inhoud"">
                                    <h5>Identiteit, perfectionisme, parentificatie.</h5>
                                    <p>Lore Vankerckhoven, Leni Raemen en Marlies Wintmolders.</p>
                                  </div>
                                </div><br />

                                <div class=""sessie"">
                                  <div class=""datum"">
                                    <span class=""dag"">9</span>
                                    <span class=""maand"">december</span>
                                  </div>
                                  <div class=""inhoud"">
                                    <h5>Lichaamsbeeld en gender.</h5>
                                    <p>Celine Vereeck, em. prof. Michel Probst en Annelies Van Den Haute.</p>
                                  </div>
                                </div><br />

                                <div class=""sessie"">
                                  <div class=""datum"">
                                    <span class=""dag"">12</span>
                                    <span class=""maand"">mei</span>
                                  </div>
                                  <div class=""inhoud"">
                                    <h5>Beweging bij eetstoornissen.</h5>
                                    <p>em. prof. Michel Probst en Nele Willems.</p>
                                  </div>
                                </div><br />

                                <div class=""sessie"">
                                  <div class=""datum"">
                                    <span class=""dag"">19</span>
                                    <span class=""maand"">mei</span>
                                  </div>
                                  <div class=""inhoud"">
                                    <h5>Hormonen en eetstoornissen.</h5>
                                    <p>Annelies Van Driessche.</p>
                                  </div>
                                </div>
                              </div>
                            </div>
                            ",
                Address = new Address("Stadsparklaan 1", "", "Gent", "9000"),
                RegisterLink = "https://www.google.com/"
            },

            new()
            {
                Title = "SOS studievoortgang en stress.",
                Date = new EventTimeSlot(
                    DateOnly.FromDateTime(now.AddDays(90 + random.Next(0, 30))),
                    new TimeOnly(14, 0)),
                ImageUrl = "event2.jpeg",
                Subject = Topics.StudentAssociation.Name,
                Content = @"
                            <div class=""infosessie-studiebegeleiding"">
                              <h3>
                                Soms loopt studeren niet helemaal zoals gepland - een vak niet gehaald, onder studievoortgangsmaatregelen
                                of niet meer weten hoe het verder moet? Tijdens deze infosessie leggen de studiebegeleider en
                                studietrajectbegeleider je stap voor stap uit:
                              </h3>

                              <ul>
                                <li>Wat studievoortgangsmaatregelen precies zijn</li>
                                <li>Wat er gebeurt als je minder opleidingsonderdelen / studiepunten behaalt dan verwacht</li>
                                <li>Hoe je prioriteiten stelt in jouw studieprogramma</li>
                                <li>Waar je terecht kunt voor hulp en advies</li>
                              </ul><br />

                              <p>
                                Of je nu wat extra uitleg wilt of gewoon voorbereid wilt zijn, iedereen is welkom.
                                Ook ouders kunnen zich voor deze digitale sessie inschrijven.
                                Opgelet, dit is een informatieve sessie – er worden geen persoonlijke dossiers besproken of behandeld.
                                Algemene vragen stellen is uiteraard wel mogelijk.
                              </p><br />

                              <h4><b>Praktisch</b></h4>
                              <p>
                                Maandag 17 november om 19 uur<br />
                                Via Teams, de link zal je op voorhand ontvangen na inschrijving.
                              </p>
                            </div>
                            ",
                Address = new Address("Stadsparklaan 1", "", "Gent", "9000"),
                RegisterLink = "https://www.google.com/",
                Price = 10.00
            },

            new()
            {
                Title = "Logopedische behandeling bij kinderen met schisis",
                Date = new EventTimeSlot(
                    DateOnly.FromDateTime(now.AddDays(120 + random.Next(0, 30))),
                    new TimeOnly(14, 0), new TimeOnly(16, 30)),
                ImageUrl = "image1.jpg",
                Subject = Topics.WellBeing.Name,
                Content = @"
                            <div class=""opleiding-schisis"">
                              <h3>
                                Wil je als logopedist je kennis uitbreiden over de behandeling van kinderen met schisis?
                                Mis je wat kennis en vaardigheid om vastberaden je eerste schisis-casus te begeleiden?
                                Wil je hands-on tools om kinderen te ondersteunen?
                                Met deze korte opleiding voel je je gesterkt om met voldoende vertrouwen te starten.
                              </h3>

                              <p>
                                Wanneer je begint met het begeleiden van kinderen met schisis maar je nog geen echte ervaring hebt,
                                kan het je al eens ontbreken aan durf om aan de slag te gaan met een casus. Misschien is het gewoon al
                                een hele tijd geleden dat je rond cleft palate werkte en wens je een opfrissing of misschien ontbreekt
                                het je gewoon aan deze specifieke know-how. Geen nood. Deze micro-credential start bij de basis en gidst
                                je stap voor stap door de essentie. Vervolgens duiken ervaren lesgevers en logopedisten en onderzoekers
                                met jou in de materie.
                              </p><br />

                              <p>
                                Van diagnostiek tot behandeling; deze opleiding neemt je aarzeling weg en helpt je om met de juiste
                                startkennis aan de slag te kunnen en met de nodige diepgang je casus verder aan te pakken.
                              </p><br />

                              <p>
                                De evaluatie gebeurt aan de hand van een casusbespreking (mondeling en schriftelijk) en een mondeling examen.
                              </p><br />

                              <h4><b>Flexibel (hybride) studeren.</b></h4>
                              <p>
                                We bieden een mix aan van online en offline leren, waardoor een deel van de leerstof op eigen tempo en in
                                eigen tijd zelfstandig kan verwerkt worden. De klassikale lessen op de campus worden efficiënt georganiseerd,
                                op een halve dag of tijdens een vakantieperiode. Op verschillende momenten wordt hybride les gegeven en kan je
                                ervoor kiezen om online aan te sluiten.
                              </p><br />

                              <p>
                                Aanwezigheid op campus wordt wel verwacht tijdens de oefensessies en hoorcolleges met gastsprekers.
                                In totaal gaat dit over 3 contactmomenten (1 volle dag en 2 halve dagen).
                              </p>
                            </div>
                            ",
                Address = new Address("Stadsparklaan 1", "", "Gent", "9000"),
            }
        };

        dbContext.Events.AddRange(eventList);
        await dbContext.SaveChangesAsync();
    }

    private async Task NewsAsync()
    {
        if (await dbContext.News.AnyAsync())
            return;

        var newsList = new List<News>
        {
            new()
            {
                Title = "Schoonmeersen C krijgt biodiversiteits­boost.",
                Subject = Topics.Education.Name,
                Date = DateTime.UtcNow.AddDays(4),
                Content = @"
                            <div class=""campus-schoonmeersen-werken"">
                              <h3>
                                Na de omgevingswerken rond gebouw C op campus Schoonmeersen krijgt de buitenruimte een volledig nieuwe invulling,
                                met multifunctioneel groen als leidraad. Het ontwerp past binnen de bredere ambitie om kerncampus Schoonmeersen te ontwikkelen
                                tot een duurzame, robuuste, leefbare en toekomstgerichte campus, waarvoor de directie Infrastructuur en Facilitair Beheer van
                                HOGENT nauw samenwerkt met de werkgroep Biodiversiteit en het onderzoekscentrum AgroFoodNature.
                              </h3>

                              <h4><b>Regenwater en bloemenpracht</b></h4><br />
                              <p>
                                Bij de rioleringswerken rond gebouw C zijn meteen ook aanpassingen doorgevoerd om regenwater optimaal te waarderen.
                                Zo zijn de dakafvoeren aangesloten op regenwaterputten voor maximaal hergebruik en is de buitenruimte verder onthard
                                en ingericht met doorlatende tegels, grasdallen en wadi’s (wat staat voor ‘water afvoer drainage en infiltratie’).
                                Die waterbuffering maakt de campus extra klimaatrobust.
                              </p><br />

                              <p>
                                Afgelopen zomervakantie werden al enkele groenzones rond gebouw C ingezaaid met een bloemrijk grasmengsel.
                                Er werden tijdelijk paaltjes geplaatst om de kiemende planten alle kansen te geven.
                                Voorbijgangers kunnen daardoor nu al genieten van het kleurrijk resultaat: rode klaprozen, blauwe korenbloemen en bernagie,
                                gele herik en witte valse kamille. Op termijn worden de paaltjes daar verwijderd en worden deze groenzones vrij toegankelijk.
                              </p><br />

                              <h4><b>Meer dan 20 bomen</b></h4><br />
                              <p>
                                De plannen werden tijdens de werken nog aangepast om enkele waardevolle bomen te behouden,
                                waaronder de eik op het kruispunt van de toegangsweg aan de Voskenslaan en het pad rond gebouw C.
                                De eik werd preventief gesnoeid en ingepakt om geen schade op te lopen van de zware machines die bij de werken gebruikt werden.
                                Dit najaar worden er nog meer dan 20 bomen extra aangeplant. Die werden zorgvuldig gekozen in overleg met de brandweer
                                en rekening houdend met de veiligheidsvoorschriften. Onder de bomen komen dynamische vaste plantenborders die met hun nectar en
                                zaden een voedselbuffet vormen voor insecten en vogels. Studenten van de opleiding groenmanagement volgen het beheer daarvan de
                                komende jaren mee op, waardoor ze er praktijkervaring opdoen in ecologisch beheer.
                              </p><br />

                              <h4><b>Aangename plek voor iedereen</b></h4><br />
                              <p>
                                Zodra ook het buitenmeubilair er een plaats krijgt, zal de omgeving rond gebouw C snel uitgroeien tot een populaire plek
                                om te vertoeven, buiten te lunchen, even te ontspannen of bij mooi weer in het bloemrijk gras te gaan liggen.
                              </p>
                            </div>
                            ",
                AuthorName = "Sofie Van den Broeck",
                AuthorFunction = "afdelingshoofd Zorg",
                AuthorAvatarUrl = null,
                ImageUrl = "image1.jpg"
            },
            new()
            {
                Title = "HOGENT is als eerste Vlaamse hogeschool SDG Ambassador",
                Subject = Topics.Sports.Name,
                Date = DateTime.UtcNow.AddDays(7),
                Content = @"
                            <div class=""sdg-ambassadeur-artikel"">
                              <h3>
                                Tijdens de Voka Dag Duurzaam Ondernemen, op donderdag 23 oktober in Technopolis Mechelen,
                                ontving HOGENT het certificaat SDG Ambassadeur, een VN-gerelateerde erkenning.
                                Dat is een bekroning voor het consequent en doorgedreven duurzaamheidsbeleid waarbij de SDG’s
                                (Sustainable Development Goals) de leidraad vormen. HOGENT is de eerste hogeschool in Vlaanderen
                                die zich SDG Ambassadeur mag noemen.
                              </h3>

                              <p>
                                De erkenning als SDG Ambassadeur is de derde en laatste stap in een ontwikkelingsstrategie op weg naar
                                een duurzaamheidsstrategie en -beleid dat HOGENT in september 2021 opstartte in samenwerking met CIFAL Flanders,
                                een trainings- en expertisecentrum op het vlak van de Sustainable Development Goals (SDG’s) dat gelieerd is aan de Verenigde Naties.
                                Tegelijkertijd is het een stimulans en opdracht om op de ingeslagen weg verder te gaan.
                              </p><br />

                              <p>
                                Het traject leidde tot verschillende acties en beleidsbeslissingen die twee jaar later, in november 2023,
                                leidden tot de erkenning als SDG Pioneer en amper een jaar nadien volgde het certificaat SDG Champion.
                                Die snelle evolutie was een gevolg van de aanhoudende en gecoördineerde inspanningen die HOGENT leverde
                                rond klimaatbestendigheid (met onder meer veel aandacht voor biodiversiteit, ontharding en waterbeheer en CO2-reductie)
                                maar evenzeer op het vlak van duurzame mobiliteit en inclusie, integriteit en mensenrechten
                                (bijvoorbeeld bij samenwerking met partners uit een aantal Afrikaanse landen waarmee HOGENT onderwijs- en onderzoeksprojecten heeft uitgebouwd).
                                Het feit dat duurzaamheid al jaren structureel verankerd zat in de onderzoekscontext, droeg eveneens bij tot de erkenningen.
                              </p><br />

                              <h4><b>Breed gedragen</b></h4><br />
                              <p>
                                HOGENT koos er ook voor om met af aan medewerkers uit zoveel mogelijk geledingen van de organisatie en studenten – die meer
                                dan het merendeel vorm geven aan de Sustainable Office – te betrekken bij het duurzaamheidsbeleid.
                                Zo wordt het beleid gedragen door de hele organisatie en vertaald naar concrete acties en initiatieven.
                              </p><br />

                              <p>
                                Bij zo’n erkenning als SDG Ambassadeur gaat het niet louter over het afvinken van een lijstje met aandachtspunten.
                                Een internationale jury hield het uitgewerkte duurzaamheidsbeleid kritisch tegen het licht en ging dieper na
                                hoe dat geïntegreerd is in de algemene organisatiestrategie en tot concrete uitvoering resulteert binnen HOGENT.
                              </p><br />

                              <h4><b>Masterplan</b></h4><br />
                              <p>
                                Het HOGENT-duurzaamheidsbeleid omvat verschillende invalshoeken en staat niet op zich,
                                maar zit verankerd in zowel alle aspecten van het beleid.
                                Zo is het aankoopbeleid gekoppeld aan het duurzaamheids- en inclusiebeleid.
                                Ook infrastructurele aanpassingen ondergaan vooraf een duurzaamheidscheck.
                                Mooie voorbeelden daarvan zijn de omgeving van gebouw T en de ingrijpende verbouwingswerken aan gebouw C op campus Schoonmeersen.
                              </p><br />

                              <p>
                                “Als je ziet hoe die infrastructuurwerken werden aangepakt, met een integratie van elementen als waterhuishouding,
                                mobiliteit en biodiversiteit, dan kan je niet anders dan vaststellen dat we heel wat tastbare veranderingen en
                                verbeteringen hebben gerealiseerd”, zegt duurzaamheidscoördinator Karen Van Bastelaere, die het SDG Ambassadeur-certificaat in ontvangst nam.
                              </p><br />

                              <p>
                                Die verwevenheid tussen infrastructuur, ruimtebeleving en duurzaamheid in al zijn aspecten wordt doorgetrokken in het
                                infrastructurele masterplan van campus Schoonmeersen dat eerder in 2025 werd goedgekeurd.
                                Dat masterplan voorziet op lange termijn onder meer in enkele bijkomende gebouwen,
                                maar evenzeer in meer ruimte voor ontmoeting en ontspanning, vergroening, biodiversiteit en aandacht voor de buurt,
                                waarbij al die aspecten niet los van elkaar staan, maar elkaar juist versterken.
                              </p>
                            </div>
                            ",
                AuthorName = "Tom De Wilde",
                AuthorFunction = "afdelingshoofd Zorg",
                AuthorAvatarUrl = null,
                ImageUrl = "event1.png"
            },

            new()
            {
                Title = "Lokale chef verovert harten met seizoensmenu",
                Subject = Topics.WellBeing.Name,
                Date = DateTime.UtcNow.AddDays(10),
                Content =
                    "Chef Jana Peeters, bekend om haar passie voor pure smaken en lokale ingrediënten, wist afgelopen weekend de harten van menig fijnproever te veroveren met haar nieuwe seizoensmenu. In het kader van de campagne ‘Eet Lokaal, Leef Bewust’ stelde ze in het buurtrestaurant van de gemeente een herfstmenu samen dat niet alleen de smaakpapillen prikkelde, maar ook een ode bracht aan duurzame voeding en korte keten. Het menu opende met een romige pompoenrisotto, afgewerkt met geroosterde zaden en een vleugje salieolie. De geur van vers gebakken brood en kruiden vulde de zaal, terwijl gasten nieuwsgierig toekeken hoe Peeters en haar team de gerechten met precisie op de borden schikten. Als hoofdgerecht serveerde ze een stoofpotje van vergeten groenten en lokaal rundsvlees, langzaam gegaard in ambachtelijke cider. De afsluiter was een luchtige appelcrumble met kaneelroom, die volgens velen ‘het perfecte herfstgevoel’ wist op te roepen. Het diner was niet zomaar een culinaire ervaring, maar ook een sociaal gebeuren. De chef had ervoor gekozen om samen te werken met lokale producenten en studenten van de hotelschool, die hielpen bij de bereiding en bediening. ‘Ik wil tonen dat eerlijke producten en lokale samenwerking leiden tot een warmere keuken én gemeenschap’, verklaarde Peeters. De zaal was tot de laatste stoel gevuld, en na afloop brak een spontaan applaus los. Veel aanwezigen spraken hun bewondering uit voor de combinatie van eenvoud en verfijning. ‘Je proeft dat alles met liefde is klaargemaakt’, zei een van de gasten. Ook de presentatie kreeg veel lof: houten serveerplanken, herfstbladeren als tafeldecoratie en warme verlichting gaven het geheel een knusse sfeer. Volgens de organisatie was het evenement in recordtijd uitverkocht, wat de groeiende belangstelling voor duurzame gastronomie illustreert. De opbrengst van de avond ging gedeeltelijk naar een lokaal project dat voedseloverschotten herverdeelt onder gezinnen in nood. Zo kreeg de avond een extra betekenis, voorbij de culinaire beleving. Voor Jana Peeters was dit een droom die werkelijkheid werd: ‘Koken met de seizoenen is meer dan een trend. Het is een manier om respect te tonen voor onze omgeving en de mensen die ze bewerken.’ De reacties op sociale media waren lovend; tientallen bezoekers deelden foto’s van hun gerechten met hashtags als #herfstsmaak en #chefjana. Door de positieve feedback kondigde Peeters aan dat ze binnenkort een reeks workshops zal organiseren waarin ze geïnteresseerden leert koken met lokale herfstproducten. De avond eindigde met warme gesprekken, geurende thee en tevreden gezichten – een bewijs dat lekker eten ook mensen dichter bij elkaar brengt.",
                AuthorName = "Pieter Claes",
                AuthorFunction = "afdelingshoofd Zorg",
                AuthorAvatarUrl = null,
                ImageUrl = "event2.jpeg"
            },

            new()
            {
                Title = "Fietstocht verbindt buurten met elkaar",
                Subject = Topics.Sports.Name,
                Date = DateTime.UtcNow.AddDays(100),
                Content =
                    "Meer dan tweehonderd inwoners stapten op zondagmorgen enthousiast op de fiets voor de jaarlijkse fietstocht die dit jaar het thema ‘Buurten Verbinden’ droeg. De tocht van ongeveer 25 kilometer leidde de deelnemers langs de mooiste plekjes van de gemeente en zorgde voor een gezellige sfeer waarin jong en oud elkaar beter leerden kennen. Aan de start op het dorpsplein heerste al vroeg een vrolijke drukte. Families, vriendengroepen, sportieve senioren en jonge kinderen verzamelden zich met kleurrijke helmen en versierde fietsen. Na een korte opwarming en welkomstwoord van de burgemeester klonk het startschot, waarna de stoet zich langzaam in beweging zette richting het platteland. Onderweg waren er verschillende halteplaatsen voorzien waar lokale verenigingen drankjes en streekproducten aanboden. Aan de hoeve van de familie De Smet konden deelnemers genieten van vers appelsap, terwijl bij het park van Molendam een muzikale tussenstop werd gehouden met een optreden van de fanfare. De organisatie had duidelijk nagedacht over veiligheid en comfort: vrijwilligers stonden op kruispunten om het verkeer te regelen, en een mobiele fietsherstelwagen reed mee voor wie pech had. De route was bovendien bewust gekozen om door zoveel mogelijk buurten te lopen, zodat bewoners elkaar konden ontmoeten en nieuwe contacten konden leggen. ‘Het is niet zomaar een sportieve activiteit, maar vooral een manier om mensen samen te brengen’, vertelde organisator Lien Van Acker. ‘Je merkt dat mensen onderweg spontaan met elkaar beginnen te praten, zeker aan de rustpunten. Dat is precies wat we wilden bereiken.’ Halverwege de tocht werd even gestopt aan het uitkijkpunt boven het kanaal, waar deelnemers een prachtig zicht hadden over de velden en de omliggende dorpen. Sommigen maakten van het moment gebruik om foto’s te nemen of een praatje te slaan met deelnemers die ze onderweg hadden ontmoet. De laatste kilometers leidden door een stuk bosrijk gebied richting het centrum, waar een groot aantal toeschouwers stond te wachten. Aan de aankomst werden alle deelnemers verwelkomd met applaus en live-muziek van een lokale band. Nadien volgde een gezamenlijke picknick in het stadspark, waar dekentjes op het gras lagen en kinderen speelden rond de fontein. Voor veel bewoners was het een ideale afsluiter van het weekend. De organisatoren spraken van een ‘overweldigend succes’. Dankzij de inzet van tientallen vrijwilligers en het zonnige weer verliep alles vlekkeloos. De politie meldde geen incidenten en de EHBO-dienst hoefde nauwelijks in te grijpen. De fietstocht zal volgens de gemeente voortaan jaarlijks plaatsvinden, telkens met een ander thema dat de samenhang tussen buurten versterkt. Deelnemers konden zich na afloop inschrijven voor een digitale nieuwsbrief met updates over nieuwe evenementen. ‘Het is mooi om te zien hoe sport en ontmoeting hand in hand gaan,’ aldus de burgemeester. ‘Deze fietstocht toont dat kleine initiatieven een groot verschil kunnen maken in het samenbrengen van onze gemeenschap.’ De dag eindigde met tevreden gezichten, vermoeide benen en een sterk gevoel van verbondenheid – precies wat de organisatie voor ogen had.",
                AuthorName = "Leen Jacobs",
                AuthorFunction = "afdelingshoofd Zorg",
                AuthorAvatarUrl = "Profielfoto_Studentenkaart.png",
                ImageUrl = "event3.png"
            },

            new()
            {
                Title = "Nieuwe generatie accountants: kruisbestuiving tussen onderwijs en praktijk",
                Subject = Topics.StudentAssociation.Name,
                Date = DateTime.UtcNow.AddDays(120),
                Content = @"
                            <div class=""postgraduaat-gecertificeerd-accountant"">
                              <h3>
                                Op woensdag 10 september vond de proclamatie plaats van de allereerste lichting studenten van het postgraduaat
                                gecertificeerd accountant. Deze opleiding, een unicum in België, is een samenwerking tussen HOGENT,
                                Universiteit Gent, Arteveldehogeschool en het ITAA, het beroepsinstituut voor accountants en belastingadviseurs.
                              </h3>

                              <p>
                                Zeventien studenten ontvingen die avond hun getuigschrift en verzilverden daarmee een reeks vrijstellingen
                                voor het schriftelijk bekwaamheidsexamen van het ITAA. Tijdens de opleiding werden ze bovendien intensief voorbereid
                                op het mondelinge luik van het examen, met bijzondere aandacht voor het klantnadviseringsgesprek.
                                De opleiding combineert wetenschappelijke diepgang met praktijkgerichte training en is specifiek ontworpen voor
                                professionals die al actief zijn in het werkveld. HOGENT is trouwens al langer vertrouwd met het opzetten van bijzondere
                                partnerschappen. Al vele jaren biedt HOGENT de bachelor-na-bachelor (banaba) Toegepaste Fiscaliteit aan,
                                die ook in partnership met het ITAA en dezelfde didactische aanpak hanteert als het postgraduaat.
                              </p><br />

                              <p>
                                Het postgraduaat speelt in op de toenemende vraag naar gecertificeerde accountants in een complexer wordende
                                fiscale en economische context. Door actuele regelgeving, digitalisering en maatschappelijke verwachtingen is er
                                meer dan ooit nood aan goed opgeleide professionals die zowel inhoudelijk als communicatief sterk staan.
                              </p><br />

                              <p>
                                Voorafgaand aan de proclamatie van het postgraduaat gecertificeerd accountant en de banaba Toegepaste Fiscaliteit
                                vond een panelgesprek plaats rond ‘De rol van de Tax Advisor &amp; Accountant in de maatschappij’,
                                met bijdragen van Bart Van Coile (ITAA), Steven Meyvaert (Baker Tilly) en professor Patricia Everaert (UGent).
                                Net afgestudeerde student Elizabet Van der Zee deelde haar ervaringen over het traject.
                              </p><br />

                              <blockquote>
                                <p>
                                  “Deze opleiding bewijst dat onderwijsinstellingen geen eiland zijn. Door intens samen te werken met het werkveld
                                  en beroepsfederaties ontstaan er mooie synergieën, waarbij ook wij als onderwijsinstellingen veel van elkaar leren.”
                                </p>
                              </blockquote><br />

                              <p>
                                Ook student Stef Moonen, Advisor Accountancy bij Vandelanotte, blikt tevreden terug:
                                “De lessen en gastcolleges boden houvast in het verwerven van de vereiste kennis,
                                terwijl de casussen complexe situaties uit de praktijk illustreerden. De opleiding was perfect te combineren met de ITAA-stage.”
                              </p><br />

                              <p>
                                Het postgraduaat is een voorbeeld van kruisbestuiving tussen het werkveld, beroepsfederaties en onderwijsinstellingen.
                                Werkgevers ondersteunen actief studenten in hun traject naar het ITAA, terwijl onderwijsinstellingen inspelen op de nood aan levenslang leren
                                en professionele ontwikkeling in een snel evoluerende sector.
                              </p><br />

                              <p>
                                Opleidingscoördinator Thomas Desmet is alvast trots op het resultaat:
                                “Deze opleiding bewijst dat onderwijsinstellingen geen eiland zijn. Door intens samen te werken
                                met het werkveld en beroepsfederaties ontstaan mooie synergieën, waarbij ook wij als onderwijsinstellingen veel van elkaar leren.
                                De lichting die hopelijk volgend jaar afstudeert, telt momenteel bijna dubbel zoveel studenten,
                                wat de nood aan deze opleiding binnen het werkveld bevestigt.”
                              </p>
                            </div>
                            ",
                AuthorName = "Jan Vermeer",
                AuthorFunction = "afdelingshoofd Zorg",
                AuthorAvatarUrl = "image1.jpg",
                ImageUrl = "event4.png"
            },
            new()
            {
                Title = "HOGENT bouwt aan duurzame kerncampus Schoonmeersen",
                Subject = Topics.StudentAssociation.Name,
                Date = DateTime.UtcNow.AddDays(60),
                Content =
                    @"
                        <div class=""campus-schoonmeersen-ontwikkeling"">
                          <h3>
                            HOGENT zet de komende jaren sterk in op de verdere ontwikkeling van campus Schoonmeersen.
                            Het masterplan, dat recent door de Stad Gent werd goedgekeurd, maakt van de site langs de Voskenslaan
                            een kerncampus van de hogeschool. De herinrichting focust op duurzaamheid, toekomstgericht onderwijs
                            en een efficiënt gebruik van de beschikbare infrastructuur.
                          </h3>

                          <p>
                            HOGENT heeft in de voorbije 30 jaar een indrukwekkend patrimonium uitgebouwd, waaronder ook een aandeel
                            onroerend erfgoed. Wil HOGENT kwalitatief onderwijs blijven aanbieden met moderne eigentijdse infrastructuur
                            en wil zij beantwoorden aan duurzaamheidsdoelstellingen, moet er grondig geïnvesteerd worden in de nieuwbouw
                            en herontwikkeling van haar campussen. De verdere ontwikkeling van campus Schoonmeersen als kerncampus is een
                            logische keuze dankzij de centrale ligging en uitstekende bereikbaarheid. Het masterplan omvat zowel het noordelijke
                            als het zuidelijke campusterrein en voorziet in een grondige optimalisatie van de gebouwen en de open ruimtes.
                          </p><br />

                          <p>Met de nieuwe plannen wil HOGENT meerdere ambities waarmaken:</p><br />

                          <ul>
                            <li>
                              <span>Samenwerking, kruisbestuiving en studentenbeleving</span><br />
                              Doordat meer opleidingen samenkomen op één campus ontstaat een dynamische leeromgeving waar studenten
                              en medewerkers elkaar makkelijker ontmoeten, ervaringen uitwisselen en innovatieve samenwerkingen opzetten
                              over de grenzen van disciplines heen. HOGENT wil inzetten op beleving en ontspanning.
                            </li><br />
                            <li>
                              <span>Duurzaamheid en klimaatneutraliteit</span><br />
                              De beschikbare ruimtes worden efficiënter benut en flexibel ingericht met focus op toekomstbestendige
                              onderwijsvormen. Herontwikkeling betekent ook activiteiten te centraliseren op één site, wat het aantal
                              verplaatsingen tussen campussen beperkt. Dit past binnen de strategie om HOGENT om tegen 2050 volledig
                              klimaatneutraal te zijn.
                            </li><br />
                            <li>
                              <span>Een groene, autovrije campus</span><br />
                              De site wordt een levend ecosysteem met aandacht voor biodiversiteit, groene corridors,
                              verlaagde verhardingsgraad en robuuste groenstructuren die leefbaarheid verhogen. Campus Schoonmeersen
                              blijft vlot bereikbaar, met duurzame mobiliteit volgens het SIOP-plan (Stappen, Trappen, Openbaar vervoer, Personenwagens).
                            </li><br />
                          </ul><br />

                          <p>
                            De transformatie wordt zichtbaar in meerdere projecten die studenten en medewerkers rechtstreeks zullen ervaren.
                            Bijvoorbeeld:
                          </p><br />

                          <ul>
                            <li>
                              <span>Campuspark Noord:</span> een groene long in het midden van de campus vervangt het parkeerplein
                              en de huidige sportterreinen. Het wordt een plek voor ontmoeting, ontspanning en biodiversiteit.
                            </li><br />
                            <li>
                              <span>Campuspark Zuid:</span> het bestaande groene buffergebied blijft vrijgevrijwaard van bebouwing
                              en wordt verder ingericht als recreatieve en educatieve ruimte. Studenten en lectoren krijgen er volop
                              mogelijkheden om te verpozen en samen te werken.
                            </li><br />
                            <li>
                              <span>Nieuwbouw U en V:</span> deze twee bijkomende gebouwen krijgen een uitgesproken onderwijsfunctie,
                              met ruimte voor studeren, eten en ontspannen.
                            </li><br />
                          </ul><br />

                          <blockquote>
                            <p>
                              “Campus Schoonmeersen wordt meer dan ooit de plek waar studeren, werken en ontspannen hand in hand gaan.
                              Dankzij de nieuwe parken en gebouwen ontstaat een groene, innovatieve leeromgeving waar studenten zich kunnen ontwikkelen
                              en tegelijk genieten van een bruisende campusbeleving.”
                            </p>
                            <footer>Rudi De Vierman, directeur Infrastructuur en Facilitair beheer van HOGENT</footer><br />
                          </blockquote><br />

                          <p>
                            De lerarenopleidingen, al 139 jaar gehuisvest op campus Ledeganck, verhuizen naar campus Schoonmeersen.
                            In een latere fase zullen ook de opleidingen van gebouw C op campus Mercator op de Henleykaai worden beëindigd.
                            Studenten en personeelsleden van die campussen zullen hun thuis vinden op de nieuwe campus.
                          </p><br />

                          <p>
                            De uitvoering van het masterplan zal gefaseerd verlopen over meerdere jaren.
                            Zo kan HOGENT haar onderwijsactiviteiten op de campussen blijven garanderen terwijl de transformaties plaatsvinden.
                            Met deze strategische keuze bevestigt HOGENT haar rol als voortrekker in duurzaam hoger onderwijs en als
                            belangrijke onderwijsinstelling in de regio Gent.
                          </p>
                        </div>
                        ",
                AuthorName = "Eva Maes",
                AuthorFunction = "afdelingshoofd Zorg",
                AuthorAvatarUrl = null,
                ImageUrl = "image1.jpg"
            }
        };

        await dbContext.News.AddRangeAsync(newsList);
        await dbContext.SaveChangesAsync();
    }

    private async Task LessonsAsync()
    {
        if (dbContext.Lessons.Any()) return;

        // Add studyfield
        // ==============
        var studfield1 = new StudyField
        {
            Name = "Toegepaste informatica",
            Departement = await dbContext.Departments.FirstAsync(d => d.Name.Contains("IT"))
        };

        dbContext.StudyFields.Add(studfield1);


        // Add courses
        // ===========
        var courses = new List<Course>
        {
            new() { Name = "Business analysis", StudyField = studfield1 },
            new() { Name = "Real-life Integrated Software Engineering", StudyField = studfield1 },
            new() { Name = "Modern Data Architectures", StudyField = studfield1 },
            new() { Name = "Bachelorproef", StudyField = studfield1 },
            new() { Name = "The IT Professional & Career Orientation", StudyField = studfield1 }
        };

        await dbContext.Courses.AddRangeAsync(courses);


        // Assign courses to Teachers
        // ==========================
        var teachers = await dbContext.Teachers.ToListAsync();

        teachers[0].AddCourse(courses[0]);
        teachers[1].AddCourse(courses[1]);
        teachers[2].AddCourse(courses[1]);
        teachers[3].AddCourse(courses[1]);
        teachers[4].AddCourse(courses[1]);
        teachers[5].AddCourse(courses[1]);
        teachers[4].AddCourse(courses[2]);
        teachers[6].AddCourse(courses[4]);
        teachers[7].AddCourse(courses[4]);
        teachers[8].AddCourse(courses[4]);
        teachers[9].AddCourse(courses[4]);

        await dbContext.SaveChangesAsync();


        // Add Lessons
        // ===========

        var classrooms = await dbContext.Classrooms.ToArrayAsync();
        var classgroups = await dbContext.Classgroups.ToArrayAsync();
        var baseLessonData = new[]
        {
            (
                Start: new DateTime(2025, 10, 21, 9, 15, 0),
                End: new DateTime(2025, 10, 21, 12, 30, 0),
                LessonType: LessonType.Hoorcollege,
                Course: courses[0],
                Classrooms: new List<Classroom> { classrooms[3] },
                Teachers: new List<Teacher> { teachers[0] },
                Classgroups: new List<ClassGroup> { classgroups[0] }
            ),
            (
                Start: new DateTime(2025, 10, 22, 8, 15, 0),
                End: new DateTime(2025, 10, 22, 12, 30, 0),
                LessonType: LessonType.Hoorcollege,
                Course: courses[1],
                Classrooms: new List<Classroom> { classrooms[17], classrooms[18], classrooms[19] },
                Teachers: new List<Teacher> { teachers[1], teachers[2], teachers[3], teachers[4], teachers[5] },
                Classgroups: new List<ClassGroup> { classgroups[0] }
            ),
            (
                Start: new DateTime(2025, 10, 22, 15, 45, 0),
                End: new DateTime(2025, 10, 22, 17, 45, 0),
                LessonType: LessonType.Hoorcollege,
                Course: courses[2],
                Classrooms: new List<Classroom> { classrooms[14] },
                Teachers: new List<Teacher> { teachers[4] },
                Classgroups: new List<ClassGroup> { classgroups[0] }
            ),
            (
                Start: new DateTime(2025, 10, 23, 8, 15, 0),
                End: new DateTime(2025, 10, 23, 12, 30, 0),
                LessonType: LessonType.Hoorcollege,
                Course: courses[1],
                Classrooms: new List<Classroom> { classrooms[17], classrooms[18], classrooms[19] },
                Teachers: new List<Teacher> { teachers[1], teachers[2], teachers[3], teachers[4], teachers[5] },
                Classgroups: new List<ClassGroup> { classgroups[0] }
            ),
            (
                Start: new DateTime(2025, 10, 23, 13, 30, 0),
                End: new DateTime(2025, 10, 23, 17, 45, 0),
                LessonType: LessonType.Hoorcollege,
                Course: courses[4],
                Classrooms: new List<Classroom> { classrooms[0] },
                Teachers: new List<Teacher> { teachers[6], teachers[7], teachers[8], teachers[9] },
                Classgroups: new List<ClassGroup> { classgroups[0] }
            ),
            (
                Start: new DateTime(2025, 10, 24, 8, 15, 0),
                End: new DateTime(2025, 10, 24, 12, 30, 0),
                LessonType: LessonType.Hoorcollege,
                Course: courses[1],
                Classrooms: new List<Classroom> { classrooms[17], classrooms[18], classrooms[19] },
                Teachers: new List<Teacher> { teachers[1], teachers[2], teachers[3], teachers[4], teachers[5] },
                Classgroups: new List<ClassGroup> { classgroups[0] }
            ),
        };


        // Repeat timetable over a few weeks
        // =================================
        var endRepeatDate = new DateTime(2025, 12, 31);
        foreach (var baseLd in baseLessonData)
        {
            var duration = baseLd.End - baseLd.Start;
            for (var start = baseLd.Start; start.Date <= endRepeatDate.Date; start = start.AddDays(7))
            {
                var end = start + duration;
                var lesson = new Lesson(start, end, baseLd.LessonType, baseLd.Course);

                foreach (var cls in baseLd.Classrooms)
                    lesson.AddClassroom(cls);

                foreach (var t in baseLd.Teachers)
                    lesson.AddTeacher(t);

                foreach (var g in baseLd.Classgroups)
                    lesson.AddClassGroup(g);

                await dbContext.Lessons.AddAsync(lesson);
            }
        }

        await dbContext.SaveChangesAsync();


        // Assign courses to Students
        // ==========================

        var students = await dbContext.Students.ToListAsync();
        var allGroups = await dbContext.Classgroups.ToListAsync();
        var allCourses = await dbContext.Courses.ToListAsync();

        foreach (var student in students)
        {
            for (var i = 0; i < 5; i++)
            {
                student.EnrollInCourse(allCourses[i], allGroups[0]);
            }
        }

        dbContext.UpdateRange(students);
        await dbContext.SaveChangesAsync();
    }

    private async Task DeadlinesAsync()
    {
        var deadline1 = new Deadline
        {
            Title = "RISE Project Proposal",
            Description = "Submit initial project proposal for RISE course",
            StartDate = new DateTime(2025, 10, 20, 12, 30, 0),
            DueDate = new DateTime(2025, 11, 1, 23, 59, 0)
        };

        var deadline2 = new Deadline
        {
            Title = "RISE Midterm Review",
            Description = "Present midterm progress for RISE project",
            StartDate = new DateTime(2025, 11, 1, 23, 59, 0),
            DueDate = new DateTime(2025, 12, 15, 12, 30, 0)
        };

        var deadline3 = new Deadline
        {
            Title = "RISE Final Submission",
            Description = "Submit final proof of concept and report",
            StartDate = new DateTime(2025, 12, 15, 12, 30, 0),
            DueDate = new DateTime(2026, 1, 31, 23, 59, 0)
        };

        var deadline4 = new Deadline
        {
            Title = "Indienen onderzoeksvoorstel",
            Description = "Formulier invullen omtrent je onderzoeksvoorstel, hoofdvraag, deelvragen,...",
            StartDate = new DateTime(2025, 11, 29, 23, 59, 0),
            DueDate = new DateTime(2026, 5, 15, 12, 30, 0)
        };


        // Assign students to courses
        // ==========================

        var students = await dbContext.Students.ToListAsync();
        var allCourses = await dbContext.Courses.ToListAsync();
        var course1 = allCourses.FirstOrDefault(c => c.Name.Contains("Real-life Integrated"));
        var course2 = allCourses.FirstOrDefault(c => c.Name.Contains("Bachelorproef"));

        if (course1 is not null)
        {
            course1.AddDeadline(deadline1);
            course1.AddDeadline(deadline2);
            course1.AddDeadline(deadline3);
        }

        if (course2 is not null)
        {
            course2.AddDeadline(deadline4);
        }

        foreach (var student in students)
        {
            deadline1.AssignStudent(student);
            deadline2.AssignStudent(student);
            deadline3.AssignStudent(student);
            deadline4.AssignStudent(student);
        }

        dbContext.Deadlines.AddRange(deadline1, deadline2, deadline3, deadline4);
        dbContext.UpdateRange(students);
        await dbContext.SaveChangesAsync();
    }

    private async Task StudentClubsAsync()
    {
        if (dbContext.StudentClubs.Any())
            return;

        var studentClubs = new List<StudentClub>
        {
            new StudentClub
            {
                Name = "Heimdal",
                Description = "Richt zich naar studenten die geïnteresseerd zijn in games en geeky stuff.",
                WebsiteUrl = "https://www.heimdal.be",
                FacebookUrl = "https://www.facebook.com/heimdal.be",
                InstagramUrl = "https://www.instagram.com/heimdal.be",
                ShieldImageUrl = "schild-heimdal.png",
                Email = new EmailAddress("gate@heimdal.be")
            },
            new StudentClub
            {
                Name = "Nemesis",
                Description = "Studentenvereniging voor board- en cardgameliefhebbers.",
                WebsiteUrl = "https://www.nemesisgent.be",
                FacebookUrl = "https://www.facebook.com/NMSSGENT/",
                InstagramUrl = "https://www.instagram.com/nemesisgent/",
                ShieldImageUrl = "schild-nemesis.png",
                Email = new EmailAddress("nemesisgent@gmail.com")
            },
            new StudentClub
            {
                Name = "Confabula",
                Description = "De studentenclub voor de studentenresidentie op campus Schoonmeersen",
                WebsiteUrl = "https://www.confabula.be",
                FacebookUrl = "https://www.facebook.com/studentenclubconfabula#",
                InstagramUrl = "https://www.instagram.com/studentenclub.confabula/",
                ShieldImageUrl = "schild-confabula.png",
                Email = new EmailAddress("info.confabula@gmail.com")
            },
            new StudentClub
            {
                Name = "Anabolica",
                Description = "Voor studenten kleuteronderwijs, lager onderwijs en secundair onderwijs.",
                WebsiteUrl = "https://www.anabolicagent.be",
                FacebookUrl = "https://www.facebook.com/anabolica/",
                InstagramUrl = "https://www.instagram.com/anabolicagent/",
                ShieldImageUrl = "schild-anabolica.png",
                Email = new EmailAddress("anabolicagent@gmail.com")
            },
            new StudentClub
            {
                Name = "Pihonia",
                Description =
                    "Pihonia is de studentenclub voor alle afstudeerrichtingen bedrijfsmanagement. Wij zijn een studentenclub in volle groei, dit zorgt voor een unieke sfeer binnen de club en voor een diepgaande vriendschapsband tussen de leden onderling.",
                WebsiteUrl = "https://pihonia.weebly.com/",
                FacebookUrl = "https://www.facebook.com/PihoniaGent",
                InstagramUrl = "https://www.instagram.com/pihonia.gent/",
                ShieldImageUrl = "schild-pihonia.png",
                Email = new EmailAddress("info@pihonia.be")
            }
        };

        dbContext.StudentClubs.AddRange(studentClubs);
        await dbContext.SaveChangesAsync();
    }

    private async Task StudentDealsAsync()
    {
        if (dbContext.StudentDeals.Any())
            return;

        var studentDeals = new List<StudentDeal>
        {
            new StudentDeal
            {
                Store = "Kinepolis Gent", Name = "Gratis popcorn", Discount = 0,
                Description = "Bij aankoop van een cinematicket, 1 gratis zak medium popcorn!",
                DueDate = DateTime.Now.AddMonths(2), PromoCategory = CategoriesPromo.Free.Name,
                WebUrl = "https://google.com", DiscountCode = "1-ABC123-456DEFG-958HIJK", ImageUrl = "kinepolis.png"
            },
            new StudentDeal
            {
                Store = "Okay", Name = "Lays deal", Discount = 15,
                Description = "15% korting op het hele assortiment van Lays in alle Okay winkels.",
                DueDate = DateTime.Now.AddMonths(1), PromoCategory = CategoriesPromo.Food.Name,
                WebUrl = "https://google.com", DiscountCode = "1-ABC123-456DEFG-958HIJK", ImageUrl = "lays.png"
            },
            new StudentDeal
            {
                Store = "CampusShop", Name = "Laptop korting", Discount = 10,
                Description = "10% korting op alle laptops gedurende de hele maand september.",
                DueDate = DateTime.Now.AddMonths(3), PromoCategory = CategoriesPromo.Tech.Name,
                WebUrl = "https://google.com", DiscountCode = "1-ABC123-456DEFG-958HIJK", ImageUrl = "hp.png"
            },
            new StudentDeal
            {
                Store = "Guido", Name = "Gratis Guido zak", Discount = 0,
                Description = "Je gratis Guido zak is op te halen in komende 3 weken op campus Schoonmeersen!",
                DueDate = DateTime.Now.AddMonths(2), PromoCategory = CategoriesPromo.Free.Name,
                WebUrl = "https://google.com", ImageUrl = "guido.png"
            },
            new StudentDeal
            {
                Store = "Domino's Pizza", Name = "Student Monday", Discount = 30,
                Description = "Elke maandag 30% korting op alle pizza's voor studenten.",
                DueDate = DateTime.Now.AddMonths(3), PromoCategory = CategoriesPromo.Food.Name,
                WebUrl = "https://google.com", ImageUrl = "dominos.jpg"
            },
            new StudentDeal
            {
                Store = "Subway", Name = "6-inch Deal", Discount = 15,
                Description = "15% korting op elke 6-inch sub op vertoon van je studentenkaart.",
                DueDate = DateTime.Now.AddMonths(1), PromoCategory = CategoriesPromo.Food.Name,
                WebUrl = "https://google.com", ImageUrl = "subway.jpg"
            },
            new StudentDeal
            {
                Store = "Coolblue", Name = "Studenten accessoires", Discount = 10,
                Description = "10% korting op laptopaccessoires zoals muizen, sleeves en toetsenborden.",
                DueDate = DateTime.Now.AddMonths(2), PromoCategory = CategoriesPromo.Tech.Name,
                DiscountCode = "1-ABC123-456DEFG-958HIJK", WebUrl = "https://google.com", ImageUrl = "coolblue.jpg"
            },
            new StudentDeal
            {
                Store = "Apple", Name = "Apple Education Pricing", Discount = 0,
                Description = "Exclusieve studentenprijzen op MacBooks en iPads via de Apple Education Store.",
                DueDate = DateTime.Now.AddMonths(4), PromoCategory = CategoriesPromo.Tech.Name,
                WebUrl = "https://google.com", ImageUrl = "apple.jpg"
            },
            new StudentDeal
            {
                Store = "Vooruit Gent", Name = "Student Tickets", Discount = 20,
                Description = "20% korting op geselecteerde cultuurvoorstellingen.",
                DueDate = DateTime.Now.AddMonths(1), PromoCategory = CategoriesPromo.Culture.Name,
                DiscountCode = "1-ABC123-456DEFG-958HIJK", WebUrl = "https://google.com", ImageUrl = "vooruit.jpg"
            },
            new StudentDeal
            {
                Store = "STAM", Name = "Studententarief", Discount = 50,
                Description = "50% korting op een ticket voor het STAM museum.", DueDate = DateTime.Now.AddMonths(2),
                PromoCategory = CategoriesPromo.Culture.Name, DiscountCode = "1-ABC123-456DEFG-958HIJK",
                WebUrl = "https://google.com", ImageUrl = "stam.jpg"
            },
            new StudentDeal
            {
                Store = "Basic-Fit", Name = "Student Fitness Pack", Discount = 10,
                Description = "10% korting op een jaarabonnement bij vertoon van studentenkaart.",
                DueDate = DateTime.Now.AddMonths(3), PromoCategory = CategoriesPromo.Sport.Name,
                WebUrl = "https://google.com", ImageUrl = "basicfit.jpg"
            },
            new StudentDeal
            {
                Store = "KRC Gent Atletiek", Name = "Student Lidgeld", Discount = 20,
                Description = "20% korting op jaarlidgeld voor studenten.", DueDate = DateTime.Now.AddMonths(2),
                PromoCategory = CategoriesPromo.Sport.Name, DiscountCode = "1-ABC123-456DEFG-958HIJK",
                WebUrl = "https://google.com", ImageUrl = "krcgent.webp"
            },
            new StudentDeal
            {
                Store = "H&M", Name = "Student Korting", Discount = 10,
                Description = "10% korting op de volledige collectie via de H&M app.",
                DueDate = DateTime.Now.AddMonths(3), PromoCategory = CategoriesPromo.Clothing.Name,
                DiscountCode = "1-ABC123-456DEFG-958HIJK", WebUrl = "https://google.com", ImageUrl = "h&m.jpg"
            },
            new StudentDeal
            {
                Store = "Zalando", Name = "Studentenkorting", Discount = 12,
                Description = "12% korting op geselecteerde kleding voor studenten.",
                DueDate = DateTime.Now.AddMonths(2), PromoCategory = CategoriesPromo.Clothing.Name,
                DiscountCode = "1-ABC123-456DEFG-958HIJK", WebUrl = "https://google.com", ImageUrl = "zalando.jpg"
            },
            new StudentDeal
            {
                Store = "IKEA", Name = "Studenten Essentials", Discount = 10,
                Description = "10% korting op woonaccessoires voor je kot.", DueDate = DateTime.Now.AddMonths(1),
                PromoCategory = CategoriesPromo.Home.Name, DiscountCode = "1-ABC123-456DEFG-958HIJK",
                WebUrl = "https://google.com", ImageUrl = "ikea.jpg"
            },
            new StudentDeal
            {
                Store = "Colruyt", Name = "Kot Essentials", Discount = 5,
                Description = "5% korting op schoonmaak- en onderhoudsproducten.", DueDate = DateTime.Now.AddMonths(2),
                PromoCategory = CategoriesPromo.Home.Name, DiscountCode = "1-ABC123-456DEFG-958HIJK",
                WebUrl = "https://google.com", ImageUrl = "colruyt.jpg"
            },
            new StudentDeal
            {
                Store = "Acco", Name = "Handboeken Deal", Discount = 10,
                Description = "10% korting op studieboeken bij Acco voor studenten.",
                DueDate = DateTime.Now.AddMonths(4), PromoCategory = CategoriesPromo.Study.Name,
                DiscountCode = "1-ABC123-456DEFG-958HIJK", WebUrl = "https://google.com", ImageUrl = "acco.jpg"
            },
            new StudentDeal
            {
                Store = "MijnErasmus", Name = "Taalcursus studenten", Discount = 20,
                Description = "20% korting op online taalcursussen Engels/Frans.", DueDate = DateTime.Now.AddMonths(2),
                PromoCategory = CategoriesPromo.Study.Name, DiscountCode = "1-ABC123-456DEFG-958HIJK",
                WebUrl = "https://google.com", ImageUrl = "erasmus.jpg"
            },
            new StudentDeal
            {
                Store = "Adobe", Name = "Gratis Creative Cloud Trial", Discount = 0,
                Description = "7 dagen gratis toegang tot Adobe Creative Cloud voor studenten.",
                DueDate = DateTime.Now.AddMonths(1), PromoCategory = CategoriesPromo.Free.Name,
                DiscountCode = "1-ABC123-456DEFG-958HIJK", WebUrl = "https://google.com", ImageUrl = "adobe.jpg"
            },
            new StudentDeal
            {
                Store = "Spotify", Name = "3 maanden gratis Premium", Discount = 0,
                Description = "Gratis Spotify Premium gedurende 3 maanden voor nieuwe studenten.",
                DueDate = DateTime.Now.AddMonths(3), PromoCategory = CategoriesPromo.Free.Name,
                DiscountCode = "1-ABC123-456DEFG-958HIJK", WebUrl = "https://google.com", ImageUrl = "spotify.jpg"
            },
            new StudentDeal
            {
                Store = "NMBS", Name = "Studenten Go Pass", Discount = 20,
                Description = "20% korting op een 10-rittenkaart voor jongeren.", DueDate = DateTime.Now.AddMonths(6),
                PromoCategory = CategoriesPromo.Other.Name, DiscountCode = "1-ABC123-456DEFG-958HIJK",
                WebUrl = "https://google.com", ImageUrl = "nmbs.jpg"
            },
            new StudentDeal
            {
                Store = "Indigo Neo", Name = "Student Parkeerkorting", Discount = 10,
                Description = "10% korting op parkeerabonnementen in Gent.", DueDate = DateTime.Now.AddMonths(2),
                PromoCategory = CategoriesPromo.Other.Name, DiscountCode = "1-ABC123-456DEFG-958HIJK",
                WebUrl = "https://google.com", ImageUrl = "indigoneo.jpg"
            },
        };

        dbContext.StudentDeals.AddRange(studentDeals);
        await dbContext.SaveChangesAsync();
    }

    private async Task JobsAsync()
    {
        if (dbContext.Jobs.Any())
            return;

        var jobs = new List<Job>
        {
            new Job
            {
                Name = "Student medewerker bibliotheek Schoonmeersen",
                CompanyName = "HOGENT",
                Description = "Je helpt aan de balie, verwerkt uitleningen en ondersteunt studenten in de leeszaal.",
                Address = new Address("Valentin Vaerwyckweg", "1", "9000", "Gent"),
                WebsiteUrl = "https://www.hogent.be/student/bibliotheken/",
                ImageUrl = "HOGENT_Logo_Zwart.png",
                JobCategory = CategoriesJob.StudentJob.Name,
                EmailAddress = new EmailAddress("bibschoonmeersen@hogent.be"),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(3),
                Salary = 14.50
            },
            new Job
            {
                Name = "IT-helpdesk assistent Rita",
                CompanyName = "HOGENT",
                Description = "Je helpt mee met eenvoudige IT-tickets, hardwarecheck en studenten ondersteuning.",
                Address = new Address("Valentin Vaerwyckweg", "1", "Gent", "9000"),
                WebsiteUrl = "https://www.hogent.be/helpdesk/",
                ImageUrl = "HOGENT_Logo_Zwart.png",
                JobCategory = CategoriesJob.StudentJob.Name,
                EmailAddress = new EmailAddress("rita@hogent.be"),
                StartDate = DateTime.Now.AddDays(7),
                EndDate = DateTime.Now.AddMonths(4),
                Salary = 15.25
            },

            new Job
            {
                Name = "Low-Code Developer stage",
                CompanyName = "Delaware",
                Description =
                    "Je werkt mee aan nieuwe features voor een SaaS-platform in een agile team, met low-code technologieën",
                Address = new Address("Amelia Earhartlaan", "10", "9000", "Gent"),
                WebsiteUrl = "https://www.delaware.pro/en-be",
                ImageUrl = "Delaware.jpg",
                JobCategory = CategoriesJob.Internship.Name,
                EmailAddress = new EmailAddress("lotte.vermoesen@delaware.be"),
                StartDate = DateTime.Now.AddDays(20),
                EndDate = DateTime.Now.AddMonths(12),
                Salary = 0
            },
            new Job
            {
                Name = "Assistent Manager",
                CompanyName = "Pokébowl",
                Description =
                    "Je zal het team effectief begeleiden en zorgen voor een soepel verloop van de dagelijkse werkzaamheden.",
                Address = new Address("Vlaanderenstraat", "116", "Gent", "9000"),
                WebsiteUrl = "https://hawaiianpokebowl.be/nl-BE/",
                ImageUrl = "HawaiianPokebowl.jpg",
                JobCategory = CategoriesJob.Fulltime.Name,
                EmailAddress = new EmailAddress("info@hawaiipokebowl.nl"),
                StartDate = new DateTime(DateTime.Now.Year, 2, 15),
                EndDate = new DateTime(DateTime.Now.Year, 6, 30),
                Salary = 16.5
            },
            new Job
            {
                Name = "Student Horeca medewerker",
                CompanyName = "Van Der Valk",
                Description = "Je helpt klanten bedienen, bereidt snacks en ondersteunt tijdens drukke momenten.",
                Address = new Address("Akkerhage", "10", "Gent", "9000"),
                WebsiteUrl = "https://www.hotelgent.be/",
                ImageUrl = "VanDerValk.jpg",
                JobCategory = CategoriesJob.StudentJob.Name,
                EmailAddress = new EmailAddress("info@gent.valk.com"),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(2),
                Salary = 13.20
            },
            new Job
            {
                Name = "Customer Support Stage",
                CompanyName = "Planet Talent",
                Description = "Je helpt klanten verder via mail en telefoon, en verwerkt supporttickets.",
                Address = new Address("Frank van Dyckelaan", "7b", "Temse", "9140"),
                WebsiteUrl = "https://www.planet-talent.com/",
                ImageUrl = "PlanetTalent.jpg",
                JobCategory = CategoriesJob.Internship.Name,
                EmailAddress = new EmailAddress("info@planet-talent.com"),
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddYears(1),
                Salary = 0
            }
        };
        dbContext.Jobs.AddRange(jobs);
        await dbContext.SaveChangesAsync();
    }
}