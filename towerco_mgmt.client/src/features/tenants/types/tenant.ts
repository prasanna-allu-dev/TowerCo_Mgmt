export interface TenantRecord {
    tenancyId: number;
    operatorId: number;
    operatorName: string;
    siteId: number;
    siteCode: string;
    siteName: string;
    countryCode: string;
    towerId: number;
    startDate: string;
    endDate: string | null;
    monthlyRent: number;
    currency: string;
    isActive: boolean;
    equipmentCount: number;
}

export interface TenantFilter {
    search?: string;
    operatorId?: string;
    state?: string;
    country?: string;
}
