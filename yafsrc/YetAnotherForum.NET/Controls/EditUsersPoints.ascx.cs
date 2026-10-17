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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Controls;

using YAF.Types.Models;

/// <summary>
/// The edit users points.
/// </summary>
public partial class EditUsersPoints : BaseUserControl
{
    /// <summary>
    /// Gets or sets the current edit user.
    /// </summary>
    /// <value>The user.</value>
    public User User { get; set; }

    /// <summary>
    /// The add points_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void AddPoints_Click(object sender, EventArgs e)
    {
        if (!this.TryGetPoints(this.txtAddPoints, "ADD_POINTS", out var points))
        {
            return;
        }

        this.GetRepository<User>().AddPoints(this.User.ID, null, points);

        this.BindData();
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        this.BindData();
    }

    /// <summary>
    /// The remove points_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void RemovePoints_Click(object sender, EventArgs e)
    {
        if (!this.TryGetPoints(this.txtRemovePoints, "REMOVE_POINTS", out var points))
        {
            return;
        }

        this.GetRepository<User>().RemovePoints(this.User.ID, null, points);
        this.BindData();
    }

    /// <summary>
    /// The set user points_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SetUserPoints_Click(object sender, EventArgs e)
    {
        if (!this.TryGetPoints(this.txtUserPoints, "SET_POINTS", out var points))
        {
            return;
        }

        this.GetRepository<User>().SetPoints(this.User.ID, points);

        this.BindData();
    }

    /// <summary>
    /// Gets the points of the field (a value that is not a number is refused).
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="labelTag">The tag of the label of the field.</param>
    /// <param name="points">The points.</param>
    /// <returns>Returns if the value is a number.</returns>
    private bool TryGetPoints(TextBox field, string labelTag, out int points)
    {
        if (int.TryParse(field.Text.Trim(), out points))
        {
            return true;
        }

        this.PageBoardContext.Notify(
            string.Format(
                this.GetText("EDIT_PROFILE", "NEED_CUSTOM_NUMBER"),
                this.GetText("ADMIN_EDITUSER", labelTag).TrimEnd(':')),
            MessageTypes.warning);

        return false;
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.ltrCurrentPoints.Text = this.txtUserPoints.Text = this.User.Points.ToString();
    }
}