using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Dashboard;
using Rise.Domain.Users;
using Rise.Persistence;
using Rise.Services.Shortcuts;
using Rise.Shared.Common;
using Rise.Shared.Shortcuts;
using Rise.Domain.Departments;

namespace Rise.Services.Tests.Shortcuts;
[Collection("IntegrationTests")]
public class ShortcutServiceTest : IAsyncLifetime
{
    public required ApplicationDbContext Db;
    public required IShortcutService ShortcutService;

    public async Task InitializeAsync()
    {
        Db = await SetupDatabase.CreateDbContextAsync();
        ShortcutService = new ShortcutService(Db);
    }

    public async Task DisposeAsync()
    {
        await Db.Database.EnsureDeletedAsync();
        await Db.DisposeAsync();
    }
    

    private Shortcut CreateShortcut(string title, string icon = "icon", string label = "label", string linkUrl = "url", ShortcutType type = ShortcutType.CommunicationAndSoftware, bool defaultForGuest = false)
    {
        return new Shortcut(title, type, icon, label, linkUrl, defaultForGuest);
    }

    private Domain.Users.Student CreateUser(string accountId = "test")
    {
        return new Domain.Users.Student
        {
            AccountId = accountId,
            Firstname = "Test",
            Lastname = "User",
            Email = new EmailAddress("test@hogent.be"),
            Birthdate = new DateTime(2000, 1, 1),
            Department = new Department
            {
                Name = "Test Dept",
                Description = "Test",
                DepartmentType = DepartmentType.Department
            },
            StudentNumber = "S123",
            PreferedCampus = "Main"
        };
    }

    [Fact]
    public async Task GetIndexAsync_ShouldReturnShortcuts()
    {
        var shortcut1 = CreateShortcut("Shortcut1");
        var shortcut2 = CreateShortcut("Shortcut2");
        Db.Shortcuts.AddRange(shortcut1, shortcut2);
        await Db.SaveChangesAsync();

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10
        };

        var result = await ShortcutService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Shortcuts.Count());
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal("Shortcut1", result.Value.Shortcuts.First().Title);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldOrderByTitleAscendingByDefault()
    {
        var shortcutA = CreateShortcut("A Shortcut");
        var shortcutC = CreateShortcut("C Shortcut");
        var shortcutB = CreateShortcut("B Shortcut");
        Db.Shortcuts.AddRange(shortcutA, shortcutC, shortcutB);
        await Db.SaveChangesAsync();
        

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10
        };

        var result = await ShortcutService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        var shortcuts = result.Value.Shortcuts.ToList();
        Assert.Equal(3, shortcuts.Count);
        Assert.Equal("A Shortcut", shortcuts[0].Title);
        Assert.Equal("B Shortcut", shortcuts[1].Title);
        Assert.Equal("C Shortcut", shortcuts[2].Title);
    }

    [Fact]
    public async Task GetIndexAsync_ShouldOrderByTitleDescending()
    {
        var shortcutA = CreateShortcut("A Shortcut");
        var shortcutC = CreateShortcut("C Shortcut");
        var shortcutB = CreateShortcut("B Shortcut");
        Db.Shortcuts.AddRange(shortcutA, shortcutC, shortcutB);
        await Db.SaveChangesAsync();

        var request = new QueryRequest.SkipTake
        {
            OrderBy = "Title",
            OrderDescending = true,
            Skip = 0,
            Take = 10
        };

        var result = await ShortcutService.GetIndexAsync(request);

        Assert.True(result.IsSuccess);
        var shortcuts = result.Value.Shortcuts.ToList();
        Assert.Equal(3, shortcuts.Count);
        Assert.Equal("C Shortcut", shortcuts[0].Title);
        Assert.Equal("B Shortcut", shortcuts[1].Title);
        Assert.Equal("A Shortcut", shortcuts[2].Title);
    }

    [Fact]
    public async Task GetDefaultShortcuts_ShouldReturnOnlyDefaultShortcuts()
    {
        var shortcut1 = CreateShortcut("Shortcut1", defaultForGuest: true);
        var shortcut2 = CreateShortcut("Shortcut2", defaultForGuest: false);
        var shortcut3 = CreateShortcut("Shortcut3", defaultForGuest: true);
        Db.Shortcuts.AddRange(shortcut1, shortcut2, shortcut3);
        await Db.SaveChangesAsync();

        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 10
        };

        var result = await ShortcutService.GetDefaultShortcuts(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Shortcuts.Count());
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Contains(result.Value.Shortcuts, s => s.Title == "Shortcut1");
        Assert.Contains(result.Value.Shortcuts, s => s.Title == "Shortcut3");
    }

    [Fact]
    public async Task GetUserShortcutsAsync_ShouldReturnUserShortcutsOrderedByPosition()
    {
        var user = CreateUser();
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var shortcut1 = CreateShortcut("Shortcut1");
        var shortcut2 = CreateShortcut("Shortcut2");
        var shortcut3 = CreateShortcut("Shortcut3");
        Db.Shortcuts.AddRange(shortcut1, shortcut2, shortcut3);
        await Db.SaveChangesAsync();

        var us1 = new UserShortcut(user.Id, shortcut1.Id, 2, "#color1");
        var us2 = new UserShortcut(user.Id, shortcut2.Id, 0, "#color2");
        var us3 = new UserShortcut(user.Id, shortcut3.Id, 1, "#color3");
        Db.UserShortcuts.AddRange(us1, us2, us3);
        await Db.SaveChangesAsync();

     
        var request = new ShortcutRequest.GetForUser
        {
            UserId = user.Id,
            Skip = 0,
            Take = 10
        };

        var result = await ShortcutService.GetUserShortcutsAsync(request);

        Assert.True(result.IsSuccess);
        var shortcuts = result.Value.Shortcuts.ToList();
        Assert.Equal(3, shortcuts.Count);
        Assert.Equal("Shortcut2", shortcuts[0].Title);
        Assert.Equal("Shortcut3", shortcuts[1].Title);
        Assert.Equal("Shortcut1", shortcuts[2].Title);
        Assert.Equal("#color2", shortcuts[0].Colour);
    }

    [Fact]
    public async Task GetUserShortcutsAsync_ShouldNotReturnDeleted()
    {
        var user = CreateUser();
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var shortcut = CreateShortcut("Shortcut");
        Db.Shortcuts.Add(shortcut);
        await Db.SaveChangesAsync();

        var us = new UserShortcut(user.Id, shortcut.Id, 0, "#color");
        us.IsDeleted = true;
        Db.UserShortcuts.Add(us);
        await Db.SaveChangesAsync();

     
        var request = new ShortcutRequest.GetForUser
        {
            UserId = user.Id,
            Skip = 0,
            Take = 10
        };

        var result = await ShortcutService.GetUserShortcutsAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Shortcuts);
    }

    [Fact]
    public async Task AddShortcutToUserAsync_ShouldAddNew()
    {
        var user = CreateUser();
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var shortcut = CreateShortcut("Shortcut");
        Db.Shortcuts.Add(shortcut);
        await Db.SaveChangesAsync();

     
        var request = new ShortcutRequest.AddToUser
        {
            UserId = user.Id,
            ShortcutId = shortcut.Id
        };

        var result = await ShortcutService.AddShortcutToUserAsync(request);

        Assert.True(result.IsSuccess);

        var us = await Db.UserShortcuts.FirstOrDefaultAsync(x => x.UserId == user.Id && x.ShortcutId == shortcut.Id);
        Assert.NotNull(us);
        Assert.False(us.IsDeleted);
        Assert.Equal(0, us.Position); // since max was -1 +1 =0
        Assert.Equal("var(--secondary-color)", us.Colour);
    }

    [Fact]
    public async Task AddShortcutToUserAsync_ShouldUndeleteExisting()
    {
        var user = CreateUser();
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var shortcut = CreateShortcut("Shortcut");
        Db.Shortcuts.Add(shortcut);
        await Db.SaveChangesAsync();

        var us = new UserShortcut(user.Id, shortcut.Id, 0, "#color");
        us.IsDeleted = true;
        Db.UserShortcuts.Add(us);
        await Db.SaveChangesAsync();

     
        var request = new ShortcutRequest.AddToUser
        {
            UserId = user.Id,
            ShortcutId = shortcut.Id
        };

        var result = await ShortcutService.AddShortcutToUserAsync(request);

        Assert.True(result.IsSuccess);

        var updatedUs = await Db.UserShortcuts.FirstAsync(x => x.UserId == user.Id && x.ShortcutId == shortcut.Id);
        Assert.False(updatedUs.IsDeleted);
    }

    [Fact]
    public async Task AddShortcutToUserAsync_ShouldReturnInvalidIfAlreadyAdded()
    {
        var user = CreateUser();
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var shortcut = CreateShortcut("Shortcut");
        Db.Shortcuts.Add(shortcut);
        await Db.SaveChangesAsync();

        var us = new UserShortcut(user.Id, shortcut.Id, 0, "#color");
        Db.UserShortcuts.Add(us);
        await Db.SaveChangesAsync();

     
        var request = new ShortcutRequest.AddToUser
        {
            UserId = user.Id,
            ShortcutId = shortcut.Id
        };

        var result = await ShortcutService.AddShortcutToUserAsync(request);

        Assert.True(result.IsInvalid());
    }

    [Fact]
    public async Task AddShortcutToUserAsync_ShouldReturnNotFoundIfShortcutNotExists()
    {
        var user = CreateUser();
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

     
        var request = new ShortcutRequest.AddToUser
        {
            UserId = user.Id,
            ShortcutId = 999
        };

        var result = await ShortcutService.AddShortcutToUserAsync(request);

        Assert.True(result.IsNotFound());
    }

    [Fact]
    public async Task RemoveShortcutFromUserAsync_ShouldRemove()
    {
        var user = CreateUser();
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var shortcut = CreateShortcut("Shortcut");
        Db.Shortcuts.Add(shortcut);
        await Db.SaveChangesAsync();

        var us = new UserShortcut(user.Id, shortcut.Id, 0, "#color");
        Db.UserShortcuts.Add(us);
        await Db.SaveChangesAsync();

     
        var request = new ShortcutRequest.RemoveFromUser
        {
            UserId = user.Id,
            ShortcutId = shortcut.Id
        };

        var result = await ShortcutService.RemoveShortcutFromUserAsync(request);

        Assert.True(result.IsSuccess);

        var updatedUs = await Db.UserShortcuts.FirstAsync(x => x.UserId == user.Id && x.ShortcutId == shortcut.Id);
        Assert.True(updatedUs.IsDeleted);
    }

    [Fact]
    public async Task RemoveShortcutFromUserAsync_ShouldReturnNotFoundIfNotExists()
    {
        var user = CreateUser();
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

     
        var request = new ShortcutRequest.RemoveFromUser
        {
            UserId = user.Id,
            ShortcutId = 999
        };

        var result = await ShortcutService.RemoveShortcutFromUserAsync(request);

        Assert.True(result.IsNotFound());
    }

    [Fact]
    public async Task RemoveShortcutFromUserAsync_ShouldReturnInvalidIfAlreadyDeleted()
    {
        var user = CreateUser();
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var shortcut = CreateShortcut("Shortcut");
        Db.Shortcuts.Add(shortcut);
        await Db.SaveChangesAsync();

        var us = new UserShortcut(user.Id, shortcut.Id, 0, "#color");
        us.IsDeleted = true;
        Db.UserShortcuts.Add(us);
        await Db.SaveChangesAsync();

     
        var request = new ShortcutRequest.RemoveFromUser
        {
            UserId = user.Id,
            ShortcutId = shortcut.Id
        };

        var result = await ShortcutService.RemoveShortcutFromUserAsync(request);

        Assert.True(result.IsInvalid());
    }

    [Fact]
    public async Task UpdateShortcutOrderForUserAsync_ShouldUpdatePositions()
    {
        var user = CreateUser();
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var shortcut1 = CreateShortcut("S1");
        var shortcut2 = CreateShortcut("S2");
        var shortcut3 = CreateShortcut("S3");
        Db.Shortcuts.AddRange(shortcut1, shortcut2, shortcut3);
        await Db.SaveChangesAsync();

        var us1 = new UserShortcut(user.Id, shortcut1.Id, 0, "#c");
        var us2 = new UserShortcut(user.Id, shortcut2.Id, 1, "#c");
        var us3 = new UserShortcut(user.Id, shortcut3.Id, 2, "#c");
        Db.UserShortcuts.AddRange(us1, us2, us3);
        await Db.SaveChangesAsync();

     
        var request = new ShortcutRequest.ChangeOrder
        {
            UserId = user.Id,
            OrderedShortcutIds = new List<int> { shortcut3.Id, shortcut1.Id, shortcut2.Id }
        };

        var result = await ShortcutService.UpdateShortcutOrderForUserAsync(request);

        Assert.True(result.IsSuccess);

        await Db.Entry(us1).ReloadAsync();
        await Db.Entry(us2).ReloadAsync();
        await Db.Entry(us3).ReloadAsync();

        Assert.Equal(1, us1.Position);
        Assert.Equal(2, us2.Position);
        Assert.Equal(0, us3.Position);
    }
    
    [Fact]
    public async Task UpdateShortcutOrderForUserAsync_ShouldReturnNotFoundIfNoShortcuts()
    {
        var user = CreateUser();
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

     
        var request = new ShortcutRequest.ChangeOrder
        {
            UserId = user.Id,
            OrderedShortcutIds = new List<int>()
        };

        var result = await ShortcutService.UpdateShortcutOrderForUserAsync(request);

        Assert.True(result.IsNotFound());
    }

    [Fact]
    public async Task UpdateUserShortcutColourAsync_ShouldUpdateColour()
    {
        var user = CreateUser();
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var shortcut = CreateShortcut("Shortcut");
        Db.Shortcuts.Add(shortcut);
        await Db.SaveChangesAsync();

        var us = new UserShortcut(user.Id, shortcut.Id, 0, "#old");
        Db.UserShortcuts.Add(us);
        await Db.SaveChangesAsync();

     
        var request = new ShortcutRequest.UpdateColour
        {
            UserId = user.Id,
            ShortcutId = shortcut.Id,
            Colour = "#new"
        };

        var result = await ShortcutService.UpdateUserShortcutColourAsync(request);

        Assert.True(result.IsSuccess);

        var updatedUs = await Db.UserShortcuts.FirstAsync(x => x.UserId == user.Id && x.ShortcutId == shortcut.Id);
        Assert.Equal("#new", updatedUs.Colour);
    }

    [Fact]
    public async Task UpdateUserShortcutColourAsync_ShouldUseDefaultIfNull()
    {
        var user = CreateUser();
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

        var shortcut = CreateShortcut("Shortcut");
        Db.Shortcuts.Add(shortcut);
        await Db.SaveChangesAsync();

        var us = new UserShortcut(user.Id, shortcut.Id, 0, "#old");
        Db.UserShortcuts.Add(us);
        await Db.SaveChangesAsync();

     
        var request = new ShortcutRequest.UpdateColour
        {
            UserId = user.Id,
            ShortcutId = shortcut.Id,
            Colour = "#FABC32"
        };

        var result = await ShortcutService.UpdateUserShortcutColourAsync(request);

        Assert.True(result.IsSuccess);

        var updatedUs = await Db.UserShortcuts.FirstAsync(x => x.UserId == user.Id && x.ShortcutId == shortcut.Id);
        Assert.Equal("#FABC32", updatedUs.Colour);
    }

    [Fact]
    public async Task UpdateUserShortcutColourAsync_ShouldReturnNotFoundIfNotExists()
    {
        var user = CreateUser();
        Db.Students.Add(user);
        await Db.SaveChangesAsync();

     
        var request = new ShortcutRequest.UpdateColour
        {
            UserId = user.Id,
            ShortcutId = 999,
            Colour = "#new"
        };

        var result = await ShortcutService.UpdateUserShortcutColourAsync(request);

        Assert.True(result.IsNotFound());
    }
}