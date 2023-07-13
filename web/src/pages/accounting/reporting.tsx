import {NextPage} from "next";
import {useTranslation} from "react-i18next";
import ContentContainer from "@/components/design/ContentContainer";
import Section from "@/components/design/Section";
import useSWR from "swr";
import {useMemo, useState} from "react";
import {AccountingReportingPeriod} from "@/libs/models";
import ReactApexChart from "react-apexcharts";
import NoSSR from 'react-no-ssr';
import {formatIsoMonth} from "@/libs/formats";
import {format, parseISO} from "date-fns";
import {getLocale} from "@/libs/localization";
import {ArrowSmallLeftIcon, ArrowSmallRightIcon} from "@heroicons/react/24/solid";

const Reporting: NextPage = () => {
    const {t} = useTranslation("accounting");

    const periods = useSWR<AccountingReportingPeriod[]>("/data/accounting/reporting/periods");
    const [year, setYear] = useState<number>(new Date().getFullYear());

    const graphData = useMemo(() => {
        let datas = periods.data ?? [];
        
        datas = datas.filter((period) => parseISO(period.startDate).getFullYear() === year);

        return {
            chart: {
                id: "income-reporting",
            },
            xaxis: {
                categories: datas.map((period) => formatIsoMonth(period.startDate)),
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
                    data: datas.map((period) => period.income),
                    color: "#00b74a",

                },
                {
                    name: t("reporting.expenses"),
                    data: datas.map((period) => 0),
                    color: "#ff5252",

                },
            ]
        } as ApexCharts.ApexOptions;

    }, [periods.data, year]);

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
                <NoSSR>
                    <ReactApexChart options={graphData} series={graphData.series} type="bar" width={"100%"}
                                    height={500}/>
                </NoSSR>
            </>
        </Section>
    </ContentContainer>;
}

export default Reporting;