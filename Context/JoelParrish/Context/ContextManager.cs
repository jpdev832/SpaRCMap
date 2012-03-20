using System;
using JoelParrish.Context.Exception;

namespace JoelParrish.Context
{
	public class ContextManager
    {
        #region events
        public delegate void ContextHandler(object sender, ContextEventArgs e);
		public event ContextHandler contextUpdate;
        #endregion
		
		private static ContextManager contextManager;
		private ContextSetting settings;
		
		protected ContextManager(ContextSetting settings)
		{
			if(settings == null)
				this.settings = new ContextSetting(); //sets defaults
			else
				this.settings = settings;
		}
		
		public static ContextManager getInstance()
		{
			if(contextManager == null)
				contextManager = new ContextManager(null);
				
			return contextManager;
		}
		
		public static ContextManager getInstance(ContextSetting settings)
		{
			if(contextManager == null)
			{
				contextManager = new ContextManager(settings);
				return contextManager;
			}
				
			throw new ContextInitException("Context settings are already set. To change settings call getInstance().getSettings()");
		}

        public ContextSetting getSettings()
        {
            return settings;
        }
		
		public void update(ContextEntity context)
		{
			//should apply threshold constraints here
			//only need to perform basic analysis for user, location, and position data
		}
		
		//should add a network listener for other context aware systems that want to communicate
	}
}