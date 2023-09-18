import {NextPage} from "next";
import {useTranslation} from "react-i18next";
import ContentContainer from "@/components/design/ContentContainer";
import Section from "@/components/design/Section";
import React, {useCallback, useEffect, useMemo, useState} from "react";
import {AccountingReportingPeriod, IncomeDetail} from "@/libs/models";
import {formatAmount, formatIsoMonth} from "@/libs/formats";
import {parseISO} from "date-fns";
import {ArrowSmallLeftIcon, ArrowSmallRightIcon} from "@heroicons/react/24/solid";

import dynamic from 'next/dynamic';
import { Toggle } from "@/components/forms/Toggle";
import { getData } from "@/libs/http";

const ReactApexChart = dynamic(() => import('react-apexcharts'), {ssr: false})

const Reporting: NextPage = () => {
    const {t} = useTranslation("accounting");

    const [year, setYear] = useState<number>(new Date().getFullYear());
    const [detailed, setDetailed] = useState<boolean>(true);
    const [currentPeriod, setCurrentPeriod] = useState<AccountingReportingPeriod[]>([]);

    const filterDetails = useCallback((details: IncomeDetail[]): IncomeDetail[] => {
        if (details && detailed === false) {
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
    
                details = sortDetails(details);
            }

        return details;
    }, [detailed]);
    
    const computeData = useCallback(async () => {
        let datas = await getData<AccountingReportingPeriod[]>("/data/accounting/reporting/periods");
        datas ??= [];

        datas = datas.filter((period) => parseISO(period.startDate).getFullYear() === year);
        datas = datas.sort((a, b) => parseISO(a.startDate).getTime() - parseISO(b.startDate).getTime());

        for (let period of datas) {
            period.details = filterDetails(period.details);
        }       

        setCurrentPeriod(datas);
    }, [year, filterDetails]);

    useEffect(() => {
        computeData();
    }, [computeData, year, detailed]);

    const graphData = useMemo(() => {

        return {
            chart: {
                id: "income-reporting",
            },
            xaxis: {
                categories: currentPeriod.map((period) => formatIsoMonth(period.startDate)),
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
                    data: currentPeriod.map((period) => period.income),
                    color: "#00b74a",
                },
                {
                    name: t("reporting.expenses"),
                    data: currentPeriod.map((period) => period.expense),
                    color: "#ff5252",
                },
            ]
        } as ApexCharts.ApexOptions;

    }, [currentPeriod, t]);

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
                    <div className={"col-start-1 w-8 m-auto text-primary"} onClick={() => setYear(year - 1)}>
                        <ArrowSmallLeftIcon/>
                    </div>

                    <h1 className={"text-center col-span-6"}>
                        {year}
                    </h1>

                    <div className={"col-start-8 1 w-8 m-auto text-primary"} onClick={() => setYear(year + 1)}>
                        <ArrowSmallRightIcon/>
                    </div>
                </div>
                <div className={"print:hidden"}>
                    <ReactApexChart options={graphData} series={graphData.series} type="bar" width={"100%"}
                                    height={500}/>
                </div>
            </>
        </Section>
        {graphData &&
            <Section>
                <h2>{t("reporting.details.title")}</h2>
                <div className="text-right print:hidden">
                    <Toggle value={detailed} className={"ml-2"} text={t("reporting.details.show")} onChange={c => setDetailed(c)}/>
                </div>
                <>
                    {currentPeriod.map((period) => <div key={period.id} className={"pb-3"}>
                        <h3>{t("reporting.period")} : {formatIsoMonth(period.startDate)}</h3>
                        <div className={"grid grid-cols-4"}>
                            <div className={"underline"}>{t("reporting.details.type")}</div>
                            <div className={"underline text-right"}>{t("reporting.details.unitPrice")}</div>
                            <div className={"underline text-right"}>{t("reporting.details.count")}</div>
                            <div className={"underline text-right"}>{t("reporting.details.total")}</div>
                            <>
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
                            </>
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

export default Reporting;