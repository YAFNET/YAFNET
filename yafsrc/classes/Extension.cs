using System;

namespace yaf
{
	public class Extension 
	{
		private BasePage	m_page;
		private string		m_sExtName, m_sExtClass, m_sExtCode;
		private	int			m_nPriority = 100;

		public void Initialize(BasePage page) 
		{
			m_page = page;
		}

		public virtual void Render(ref System.Text.StringBuilder output) 
		{
		}

		public string Class
		{
			set 
			{
				m_sExtClass = value;
			}
			get 
			{
				return m_sExtClass;
			}
		}

		public string Name
		{
			set 
			{
				m_sExtName = value;
			}
			get 
			{
				return m_sExtName;
			}
		}

		public string Code 
		{
			set 
			{
				m_sExtCode = value;
			}
			get 
			{
				return m_sExtCode;
			}
		}

		public int Priority 
		{
			set 
			{
				m_nPriority = value;
			}
			get 
			{
				return m_nPriority;
			}
		}

		public BasePage Page 
		{
			get 
			{
				return m_page;
			}
		}
	}
}
