namespace VoxelEditor.MVCInterfaces
{
    /// <summary>
    /// Constructor has to take parameter of type IViewRegistry for the initialisation process in the Controller
    /// </summary>
    public abstract class ViewRegistryContainer
    {
        protected ViewRegistryContainer(IViewRegistry registry)
        {
            
        }
    }
}
