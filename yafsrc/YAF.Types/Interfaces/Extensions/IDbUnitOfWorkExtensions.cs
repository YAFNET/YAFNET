/* Yet Another Forum.net
 * Copyright (C) 2006-2011 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Types.Interfaces
{
	#region Using

	using System.Data;

	#endregion

	/// <summary>
	/// The db unit of work extensions.
	/// </summary>
	public static class IDbUnitOfWorkExtensions
	{
		#region Public Methods

		/// <summary>
		/// The commit.
		/// </summary>
		/// <param name="unitOfWork">
		/// The unit of work.
		/// </param>
		public static void Commit([NotNull] this IDbUnitOfWork unitOfWork)
		{
			CodeContracts.ArgumentNotNull(unitOfWork, "unitOfWork");

			if (unitOfWork.Transaction != null)
			{
				unitOfWork.Transaction.Commit();
			}
		}

		/// <summary>
		/// The rollback.
		/// </summary>
		/// <param name="unitOfWork">
		/// The unit of work.
		/// </param>
		public static void Rollback([NotNull] this IDbUnitOfWork unitOfWork)
		{
			CodeContracts.ArgumentNotNull(unitOfWork, "unitOfWork");

			if (unitOfWork.Transaction != null)
			{
				unitOfWork.Transaction.Rollback();
			}
		}

		/// <summary>
		/// The setup.
		/// </summary>
		/// <param name="unitOfWork">
		/// The unit of work.
		/// </param>
		/// <param name="command">
		/// The command.
		/// </param>
		public static void Setup([NotNull] this IDbUnitOfWork unitOfWork, [NotNull] IDbCommand command)
		{
			CodeContracts.ArgumentNotNull(unitOfWork, "unitOfWork");
			CodeContracts.ArgumentNotNull(command, "command");

			command.Connection = unitOfWork.Transaction.Connection;
			command.Transaction = unitOfWork.Transaction;
		}

		#endregion
	}
}