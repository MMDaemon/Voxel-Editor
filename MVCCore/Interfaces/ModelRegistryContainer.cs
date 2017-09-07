namespace MVCCore.Interfaces
{
    /// <summary>
    /// Constructor has to take parameter of type IModelRegistry for the initialisation process in the Controller
    /// </summary>
    public abstract class ModelRegistryContainer
    {
        protected ModelRegistryContainer(IModelRegistry registry)
        {
            
        }
    }
}
