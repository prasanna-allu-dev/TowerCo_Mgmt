import type { SiteStatus } from './site';
import type { WorkOrder } from './workOrder';
import type { HEALTH_STATUSES, SENSOR_TYPES } from '@/constants/health';

export type HealthStatus = (typeof HEALTH_STATUSES)[number];
export type SensorType = (typeof SENSOR_TYPES)[number];

export interface HealthSignal {
    name: string;
    status: HealthStatus;
    reason: string;
}

export interface SiteHealthSummary {
    siteId: number;
    siteCode: string;
    siteName: string;
    countryCode: string;
    siteStatus: SiteStatus;
    overall: HealthStatus;
    mainReason: string;
    signals: HealthSignal[];
}

export interface PortfolioHealth {
    healthy: number;
    warning: number;
    critical: number;
    sites: SiteHealthSummary[];
}

export interface ReadingPoint {
    recordedAtUtc: string;
    value: number;
}

export interface SensorSeries {
    sensorType: SensorType;
    latest: number;
    status: HealthStatus;
    warning: number;
    critical: number;
    direction: 'HighIsBad' | 'LowIsBad';
    points: ReadingPoint[];
}

export interface Inspection {
    id: number;
    inspectedOn: string;
    inspector: string;
    outcome: 'Pass' | 'PassWithActions' | 'Fail';
    notes: string | null;
}

export interface SiteHealth {
    summary: SiteHealthSummary;
    telemetry: SensorSeries[];
    inspections: Inspection[];
    openWorkOrders: WorkOrder[];
}

export interface HealthFilter {
    country?: string;
    status?: string;
}
