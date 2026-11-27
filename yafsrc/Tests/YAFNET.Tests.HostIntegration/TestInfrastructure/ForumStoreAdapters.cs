/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.HostIntegration.TestInfrastructure;

using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Gives the forum the users of the host application as its own identity model.
/// Every read returns a new instance, changes are only kept by <see cref="UpdateAsync"/>.
/// </summary>
public class ForumUserStoreAdapter(HostDbContext db) : IUserPasswordStore<AspNetUsers>,
                                                       IUserEmailStore<AspNetUsers>,
                                                       IUserSecurityStampStore<AspNetUsers>,
                                                       IUserRoleStore<AspNetUsers>,
                                                       IQueryableUserStore<AspNetUsers>
{
    // the key of the host is the key of the identity
    private readonly static Expression<Func<HostUser, AspNetUsers>> ToForumUser = user => new AspNetUsers
    {
        Id = user.Id.ToString(),
        UserName = user.UserName,
        NormalizedUserName = user.NormalizedUserName,
        Email = user.Email,
        NormalizedEmail = user.NormalizedEmail,
        EmailConfirmed = user.EmailConfirmed,
        PasswordHash = user.PasswordHash,
        SecurityStamp = user.SecurityStamp,
        IsApproved = user.IsApproved
    };

    public IQueryable<AspNetUsers> Users => db.Users.AsNoTracking().Select(ToForumUser);

    public async Task<IdentityResult> CreateAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        var hostUser = new HostUser();

        CopyToHostUser(user, hostUser);

        db.Users.Add(hostUser);

        await db.SaveChangesAsync(cancellationToken);

        user.Id = hostUser.Id.ToString();

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        var hostUser = await db.Users.FindAsync([int.Parse(user.Id)], cancellationToken);

        if (hostUser == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "The user does not exist." });
        }

        CopyToHostUser(user, hostUser);

        await db.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        var userId = int.Parse(user.Id);

        await db.Users.Where(u => u.Id == userId).ExecuteDeleteAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public Task<AspNetUsers> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return int.TryParse(userId, out var id)
                   ? db.Users.AsNoTracking().Where(u => u.Id == id).Select(ToForumUser)
                       .FirstOrDefaultAsync(cancellationToken)
                   : Task.FromResult<AspNetUsers>(null);
    }

    public Task<AspNetUsers> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return db.Users.AsNoTracking().Where(u => u.NormalizedUserName == normalizedUserName).Select(ToForumUser)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<AspNetUsers> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return db.Users.AsNoTracking().Where(u => u.NormalizedEmail == normalizedEmail).Select(ToForumUser)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<string> GetUserIdAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id);
    }

    public Task<string> GetUserNameAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public Task SetUserNameAsync(AspNetUsers user, string userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;

        return Task.CompletedTask;
    }

    public Task<string> GetNormalizedUserNameAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedUserName);
    }

    public Task SetNormalizedUserNameAsync(AspNetUsers user, string normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;

        return Task.CompletedTask;
    }

    public Task SetPasswordHashAsync(AspNetUsers user, string passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;

        return Task.CompletedTask;
    }

    public Task<string> GetPasswordHashAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash != null);
    }

    public Task SetEmailAsync(AspNetUsers user, string email, CancellationToken cancellationToken)
    {
        user.Email = email;

        return Task.CompletedTask;
    }

    public Task<string> GetEmailAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.EmailConfirmed);
    }

    public Task SetEmailConfirmedAsync(AspNetUsers user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;

        return Task.CompletedTask;
    }

    public Task<string> GetNormalizedEmailAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedEmail);
    }

    public Task SetNormalizedEmailAsync(AspNetUsers user, string normalizedEmail, CancellationToken cancellationToken)
    {
        user.NormalizedEmail = normalizedEmail;

        return Task.CompletedTask;
    }

    public Task SetSecurityStampAsync(AspNetUsers user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;

        return Task.CompletedTask;
    }

    public Task<string> GetSecurityStampAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.SecurityStamp);
    }

    public async Task AddToRoleAsync(AspNetUsers user, string roleName, CancellationToken cancellationToken)
    {
        var role = await db.Roles.SingleOrDefaultAsync(r => r.NormalizedName == roleName, cancellationToken)
                   ?? throw new InvalidOperationException($"The role {roleName} does not exist.");

        db.UserRoles.Add(new IdentityUserRole<int> { UserId = int.Parse(user.Id), RoleId = role.Id });

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveFromRoleAsync(AspNetUsers user, string roleName, CancellationToken cancellationToken)
    {
        await this.UserRoles(user, roleName).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IList<string>> GetRolesAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        var userId = int.Parse(user.Id);

        return await db.UserRoles.Where(ur => ur.UserId == userId)
                   .Join(db.Roles, ur => ur.RoleId, r => r.Id, (_, r) => r.Name)
                   .ToListAsync(cancellationToken);
    }

    public Task<bool> IsInRoleAsync(AspNetUsers user, string roleName, CancellationToken cancellationToken)
    {
        return this.UserRoles(user, roleName).AnyAsync(cancellationToken);
    }

    public async Task<IList<AspNetUsers>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        return await db.Roles.Where(r => r.NormalizedName == roleName)
                   .Join(db.UserRoles, r => r.Id, ur => ur.RoleId, (_, ur) => ur.UserId)
                   .Join(db.Users.AsNoTracking(), userId => userId, u => u.Id, (_, u) => u)
                   .Select(ToForumUser)
                   .ToListAsync(cancellationToken);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private static void CopyToHostUser(AspNetUsers user, HostUser hostUser)
    {
        hostUser.UserName = user.UserName;
        hostUser.NormalizedUserName = user.NormalizedUserName;
        hostUser.Email = user.Email;
        hostUser.NormalizedEmail = user.NormalizedEmail;
        hostUser.EmailConfirmed = user.EmailConfirmed;
        hostUser.PasswordHash = user.PasswordHash;
        hostUser.SecurityStamp = user.SecurityStamp;
        hostUser.IsApproved = user.IsApproved;
    }

    private IQueryable<IdentityUserRole<int>> UserRoles(AspNetUsers user, string normalizedRoleName)
    {
        var userId = int.Parse(user.Id);

        return db.UserRoles.Where(
            ur => ur.UserId == userId && db.Roles.Any(r => r.Id == ur.RoleId && r.NormalizedName == normalizedRoleName));
    }
}

/// <summary>
/// Gives the forum the roles of the host application as its own identity model.
/// </summary>
public class ForumRoleStoreAdapter(HostDbContext db) : IQueryableRoleStore<AspNetRoles>
{
    private readonly static Expression<Func<HostRole, AspNetRoles>> ToForumRole = role => new AspNetRoles
    {
        Id = role.Id.ToString(), Name = role.Name, NormalizedName = role.NormalizedName
    };

    public IQueryable<AspNetRoles> Roles => db.Roles.AsNoTracking().Select(ToForumRole);

    public async Task<IdentityResult> CreateAsync(AspNetRoles role, CancellationToken cancellationToken)
    {
        var hostRole = new HostRole { Name = role.Name, NormalizedName = role.NormalizedName };

        db.Roles.Add(hostRole);

        await db.SaveChangesAsync(cancellationToken);

        role.Id = hostRole.Id.ToString();

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(AspNetRoles role, CancellationToken cancellationToken)
    {
        var hostRole = await db.Roles.FindAsync([int.Parse(role.Id)], cancellationToken);

        if (hostRole == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "The role does not exist." });
        }

        hostRole.Name = role.Name;
        hostRole.NormalizedName = role.NormalizedName;

        await db.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(AspNetRoles role, CancellationToken cancellationToken)
    {
        var roleId = int.Parse(role.Id);

        await db.Roles.Where(r => r.Id == roleId).ExecuteDeleteAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public Task<AspNetRoles> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        return int.TryParse(roleId, out var id)
                   ? db.Roles.AsNoTracking().Where(r => r.Id == id).Select(ToForumRole)
                       .FirstOrDefaultAsync(cancellationToken)
                   : Task.FromResult<AspNetRoles>(null);
    }

    public Task<AspNetRoles> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        return db.Roles.AsNoTracking().Where(r => r.NormalizedName == normalizedRoleName).Select(ToForumRole)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<string> GetRoleIdAsync(AspNetRoles role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Id);
    }

    public Task<string> GetRoleNameAsync(AspNetRoles role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Name);
    }

    public Task SetRoleNameAsync(AspNetRoles role, string roleName, CancellationToken cancellationToken)
    {
        role.Name = roleName;

        return Task.CompletedTask;
    }

    public Task<string> GetNormalizedRoleNameAsync(AspNetRoles role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.NormalizedName);
    }

    public Task SetNormalizedRoleNameAsync(AspNetRoles role, string normalizedName, CancellationToken cancellationToken)
    {
        role.NormalizedName = normalizedName;

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
