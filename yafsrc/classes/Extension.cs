using System;

namespace yaf
{
	class Extension 
	{
		private BasePage	m_page;

		public void Initialize(BasePage page) 
		{
			m_page = page;
		}

		public virtual void Render(ref System.Text.StringBuilder output) 
		{
		}
	}
}
