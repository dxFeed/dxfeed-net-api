using System.Globalization;
using com.dxfeed.api;
using com.dxfeed.native.api;

namespace com.dxfeed.native.events {
	public struct NativeProfile : IDxProfile {
		private readonly DxProfile profile;
		private readonly string desc;

		internal unsafe NativeProfile(DxProfile* profile) {
			this.profile = *profile;
			desc = new string((char*)this.profile.description.ToPointer());
		}

		public override string ToString() {
			return string.Format(CultureInfo.InvariantCulture, "Profile: {{Description: '{0}'}}", Description);
		}

		#region Implementation of IDxProfile

		public string Description {
			get { return desc; }
		}

		#endregion
	}
}