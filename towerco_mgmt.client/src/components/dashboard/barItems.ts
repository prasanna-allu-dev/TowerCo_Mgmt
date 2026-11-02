import { COUNTRIES } from '@/constants/domain';
import type { PortfolioDashboard, SiteStatus } from '@/types';
import { statusSeverity, utilisationSeverity } from '@/utils/severity';
import type { BarItem } from './BarList';

const countryName = (code: string) => COUNTRIES.find(c => c.code === code)?.name ?? code;

export const statusBars = (d: PortfolioDashboard): BarItem[] =>
    Object.entries(d.sitesByStatus).map(([status, count]) => ({
        key: status, label: status, value: count, tone: statusSeverity(status as SiteStatus),
    }));

export const countryBars = (d: PortfolioDashboard): BarItem[] =>
    d.utilisationByCountry.map(c => ({
        key: c.countryCode, label: countryName(c.countryCode), value: c.utilisationPercent,
        display: `${c.utilisationPercent}% (${c.occupiedSlots}/${c.totalSlots})`,
        tone: utilisationSeverity(c.utilisationPercent),
    }));

export const operatorBars = (d: PortfolioDashboard): BarItem[] =>
    d.operatorShares.map(o => ({ key: String(o.operatorId), label: o.operatorName, value: o.activeTenancies }));
