// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.Management.ResourceManager.Fluent.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// Enumeration of the Azure datacenter regions. See https://azure.microsoft.com/regions/
    /// </summary>
    public partial class Region
    {
        private static ConcurrentDictionary<string, Region> regions = new ConcurrentDictionary<string, Region>();

        #region Americas
        public static readonly Region USWest = new Region("westus");
        public static readonly Region USWest2 = new Region("westus2");
        public static readonly Region USCentral = new Region("centralus");
        public static readonly Region USEast = new Region("eastus");
        public static readonly Region USEast2 = new Region("eastus2");
        public static readonly Region USNorthCentral = new Region("northcentralus");
        public static readonly Region USSouthCentral = new Region("southcentralus");
        public static readonly Region USWestCentral = new Region("westcentralus");
        public static readonly Region CanadaCentral = new Region("canadacentral");
        public static readonly Region CanadaEast = new Region("canadaeast");
        public static readonly Region BrazilSouth = new Region("brazilsouth");
        #endregion

        #region Europe
        public static readonly Region EuropeNorth = new Region("northeurope");
        public static readonly Region EuropeWest = new Region("westeurope");
        public static readonly Region UKSouth = new Region("uksouth");
        public static readonly Region UKWest = new Region("ukwest");
        public static readonly Region FranceCentral = new Region("francecentral");
        public static readonly Region FranceSouth = new Region("francesouth");
        public static readonly Region SwitzerlandNorth = new Region("switzerlandnorth");
        public static readonly Region SwitzerlandWest = new Region("switzerlandwest");
        public static readonly Region GermanyNorth = new Region("germanynorth");
        public static readonly Region GermanyWestCentral = new Region("germanywestcentral");
        public static readonly Region NorwayWest = new Region("norwaywest");
        public static readonly Region NorwayEast = new Region("norwayeast");
        #endregion

        #region Asia
        public static readonly Region AsiaEast = new Region("eastasia");
        public static readonly Region AsiaSouthEast = new Region("southeastasia");
        public static readonly Region JapanEast = new Region("japaneast");
        public static readonly Region JapanWest = new Region("japanwest");
        public static readonly Region AustraliaEast = new Region("australiaeast");
        public static readonly Region AustraliaSouthEast = new Region("australiasoutheast");
        public static readonly Region AustraliaCentral = new Region("australiacentral");
        public static readonly Region AustraliaCentral2 = new Region("australiacentral2");
        public static readonly Region IndiaCentral = new Region("centralindia");
        public static readonly Region IndiaSouth = new Region("southindia");
        public static readonly Region IndiaWest = new Region("westindia");
        public static readonly Region KoreaSouth = new Region("koreasouth");
        public static readonly Region KoreaCentral = new Region("koreacentral");
        #endregion

        #region Middle East and Africa
        public static readonly Region UAECentral = new Region("uaecentral");
        public static readonly Region UAENorth = new Region("uaenorth");
        public static readonly Region SouthAfricaNorth = new Region("southafricanorth");
        public static readonly Region SouthAfricaWest = new Region("southafricawest");
        #endregion

        #region China
        public static readonly Region ChinaNorth = new Region("chinanorth");
        public static readonly Region ChinaEast = new Region("chinaeast");
        public static readonly Region ChinaNorth2 = new Region("chinanorth2");
        public static readonly Region ChinaEast2 = new Region("chinaeast2");
        #endregion

        #region German
        public static readonly Region GermanyCentral = new Region("germanycentral");
        public static readonly Region GermanyNorthEast = new Region("germanynortheast");
        #endregion

        #region Government Cloud
        /// <summary>
        /// U.S. government cloud in Virginia.
        /// </summary>
        public static readonly Region GovernmentUSVirginia = new Region("usgovvirginia");

        /// <summary>
        /// U.S. government cloud in Iowa.
        /// </summary>
        public static readonly Region GovernmentUSIowa = new Region("usgoviowa");

        /// <summary>
        /// U.S. government cloud in Arizona.
        /// </summary>
        public static readonly Region GovernmentUSArizona = new Region("usgovarizona");

        /// <summary>
        /// U.S. government cloud in Texas.
        /// </summary>
        public static readonly Region GovernmentUSTexas = new Region("usgovtexas");

        /// <summary>
        /// U.S. Department of Defense cloud - East.
        /// </summary>
        public static readonly Region GovernmentUSDodEast = new Region("usdodeast");

        /// <summary>
        /// U.S. Department of Defense cloud - Central.
        /// </summary>
        public static readonly Region GovernmentUSDodCentral = new Region("usdodcentral");

        #endregion

        public static IReadOnlyCollection<Region> Values
        {
            get
            {
                return regions.Values as IReadOnlyCollection<Region>;
            }
        }

        public string Name
        {
            get; private set;
        }

        private Region(string name)
        {
            Name = name.ToLowerInvariant();
            regions.AddOrUpdate(Name, this, (k, v) => v);
        }

        public static Region Create(string name)
        {
            name = name.Replace(" ", "").ToLowerInvariant();
            Region region = null;
            if (regions.TryGetValue(name, out region))
            {
                return region;
            }
            return new Region(name);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public static bool operator ==(Region lhs, Region rhs)
        {
            if (object.ReferenceEquals(lhs, null))
            {
                return object.ReferenceEquals(rhs, null);
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Region lhs, Region rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Region))
            {
                return false;
            }

            if (object.ReferenceEquals(obj, this))
            {
                return true;
            }
            Region rhs = (Region)obj;
            if (Name == null)
            {
                return rhs.Name == null;
            }
            return Name.Equals(rhs.Name, System.StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}