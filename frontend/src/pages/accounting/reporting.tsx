import {useTranslation} from "react-i18next";
import ContentContainer from "@/components/design/ContentContainer";
import Section from "@/components/design/Section";
import React from "react";
import {AccountingReportingPeriod, IncomeDetail} from "@/libs/models";
import {formatAmount, formatIsoMonth} from "@/libs/formats";
import {ArrowSmallLeftIcon, ArrowSmallRightIcon} from "@heroicons/react/24/solid";

import { Toggle } from "@/components/forms/Toggle";
import { getData } from "@/libs/http";
import { Memo, useComputed, useObservable, useObserveEffect } from "@legendapp/state/react";
import ReactApexChart from 'react-apexcharts';


const AccountingReporting = () => {
    const {t} = useTranslation("accounting");
    
    const year$ = useObservable<number>(new Date().getFullYear());
    
    const detailed$ = useObservable(false);

    const currentPeriod$ = useObservable<AccountingReportingPeriod[]>([]);
    const currentPeriod = currentPeriod$.use();
    
    useObserveEffect(async () => {
        await refreshData(year$.get(), detailed$.get());
    });

    async function refreshData(year: number, detailed: boolean) {
        let datas = await getData<AccountingReportingPeriod[]>("/data/accounting/reporting/periods/" + year);
        datas ??= [];

        for (let period of datas) {
            period.details = filterDetails(period.details, detailed);
        }

        currentPeriod$.set(datas);
    }

    function  filterDetails (details: IncomeDetail[], detailed: boolean): IncomeDetail[] {
        if (details && !detailed) {
                details = details.reduce((acc, detail) => {
                    const index = acc.findIndex((d) => d.type === detail.type && d.category === detail.category);
    
                    if (index === -1) {
                        detail.count = 1;
                        detail.unitPrice = detail.subTotal;
                        acc.push({...detail});
                    } else {
                        acc[index].subTotal += detail.subTotal;
                        acc[index].unitPrice = acc[index].subTotal;
                    }
    
                    return acc;
                }, [] as IncomeDetail[]);
    
            }

        return sortDetails(details);
    }
    
    const graphData$ = useComputed(() => {

        return {
            chart: {
                id: "income-reporting",
            },
            xaxis: {
                categories: currentPeriod$.get().map((period) => formatIsoMonth(period.startDate)),
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '55%',
                    endingShape: 'rounded'
                },
            },
            stroke: {
                show: true,
                width: 2,
                colors: ['transparent']
            },
            legend: {
                show: true,
            },
            series: [
                {
                    name: t("reporting.income"),
                    data: currentPeriod$.get().map((period) => period.income),
                    color: "#00b74a",
                },
                {
                    name: t("reporting.expenses"),
                    data: currentPeriod$.get().map((period) => period.expense),
                    color: "#ff5252",
                },
            ]
        } as any;

    });
    const graphData = graphData$.use();

    function getType(detail: IncomeDetail) {
        
        if (detail.category?.length) {
            return detail.category;
        }
        
        switch (detail.type) {
            case "appointment":
                return t("reporting.details.appointments");
            case "document-expense":
                return t("reporting.details.document-expense");
            case "document-income":
                return t("reporting.details.document-income");
                
        }

        return detail.type;
    }

    function sortDetails(details: IncomeDetail[]): IncomeDetail[] {
        return details.sort((a, b) => (a.unitPrice > 0 || b.unitPrice > 0 ? b.unitPrice - a.unitPrice : a.unitPrice - b.unitPrice)
        || (a.type?.localeCompare(b?.type ?? "") 
        ?? (a.category?.localeCompare(b?.category ?? "")) ?? 0));
    }

    return <ContentContainer>
        <Section>
            <h1>{t("reporting.title")}</h1>
            <>
                <div className={"grid grid-cols-8 cursor-pointer"}>
                    <div className={"col-start-1 w-8 m-auto text-primary"} onClick={() => year$.set(year$.get() - 1)}>
                        <ArrowSmallLeftIcon/>
                    </div>

                    <h1 className={"text-center col-span-6"}>
                        <Memo>{year$}</Memo>
                    </h1>

                    <div className={"col-start-8 1 w-8 m-auto text-primary"} onClick={() => year$.set(year$.get() + 1)}>
                        <ArrowSmallRightIcon/>
                    </div>
                </div>
                <div className={"print:hidden"}>
                    <ReactApexChart options={graphData} series={graphData.series} type="bar" width={"100%"}
                                    height={500}/>
                </div>
            </>
        </Section>
        {currentPeriod &&
            <Section>
                <h2>{t("reporting.details.title")}</h2>
                <div className="text-right print:hidden">
                    <Toggle value={detailed$.get()} className={"ml-2"} text={t("reporting.details.show")} onChange={detailed$.toggle}/>
                </div>
                <>
                    {currentPeriod.map((period) => <div key={period.id} className={"pb-3"}>
                        <h3>{t("reporting.period")} : {formatIsoMonth(period.startDate)}</h3>
                        <div className={"grid grid-cols-4"}>
                            <div className={"underline"}>{t("reporting.details.type")}</div>
                            <div className={"underline text-right"}>{t("reporting.details.unitPrice")}</div>
                            <div className={"underline text-right"}>{t("reporting.details.count")}</div>
                            <div className={"underline text-right"}>{t("reporting.details.total")}</div>
                            {period.details.map((detail) => <React.Fragment key={detail.id}>
                                <div>
                                    {getType(detail)}
                                </div>
                                <div className={"text-right"}>
                                    {formatAmount(detail.unitPrice)} &euro;
                                </div>
                                <div className={"text-right"}>
                                    {detail.count}
                                </div>
                                <div className={"text-right"}>
                                    {formatAmount(detail.subTotal)} &euro;
                                </div>
                            </React.Fragment>)}
                        </div>
                        <div className={"grid grid-cols-4"}>
                            <div />
                            <div />
                            <div />
                            <div className={"text-right font-bold border-t"}>
                                {formatAmount(period.income - period.expense)} &euro;
                            </div>
                        </div>
                    </div>)}
                </>
            </Section>
        }
    </ContentContainer>;
}

export default AccountingReporting;