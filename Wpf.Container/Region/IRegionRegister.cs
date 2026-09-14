using EyeContainer.Core.Interfaces;
using System.Windows.Controls;

namespace Wpf.Container.Region
{
    public interface IRegionRegister
    {
        public void RegisterRegion(string regionName, ContentControl control);
    }

}
