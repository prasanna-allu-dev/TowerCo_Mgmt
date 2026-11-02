import type { SiteStatus } from './site';

export interface CountryUtilisation {
    countryCode: string;
    siteCount: number;
    occupiedSlots: number;
    totalSlots: number;
    utilisationPercent: number;
}

export interface CurrencyAmount {
    currency: string;
    amount: number;
}

export interface OperatorShare {
    operatorId: number;
    operatorName: string;
    activeTenancies: number;
}

export interface ExpiringTenancy {
    tenancyId: number;
    siteId: number;
    siteCode: string;
    operatorName: string;
    endDate: string;
    daysRemaining: number;
}

export interface SellableTower {
    siteId: number;
    siteCode: string;
    siteName: string;
    countryCode: string;
    availableSlots: number;
}

export interface PortfolioKpis {
    totalSites: number;
    activeSites: number;
    totalSlots: number;
    occupiedSlots: number;
    utilisationPercent: number;
    fullTowers: number;
    emptyTowers: number;
}

export interface PortfolioDashboard {
    kpis: PortfolioKpis;
    sitesByStatus: Record<SiteStatus, number>;
    utilisationByCountry: CountryUtilisation[];
    monthlyRentByCurrency: CurrencyAmount[];
    operatorShares: OperatorShare[];
    expiringTenancies: ExpiringTenancy[];
    sellableTowers: SellableTower[];
    alerts: DashboardAlert[];
}

export type Severity = 'ok' | 'warning' | 'critical' | 'neutral';

export interface DashboardAlert {
    id: string;
    severity: Severity;
    title: string;
    detail: string;
    siteId?: number;
}
