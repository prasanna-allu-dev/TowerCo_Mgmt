export interface Tenancy {
    id: number;
    operatorId: number;
    operatorName: string;
    startDate: string;
    endDate: string | null;
    monthlyRent: number;
    currency: string;
}

export interface CreateTenancyRequest {
    operatorId: number;
    startDate: string;
    monthlyRent: number;
    currency: string;
}
