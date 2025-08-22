import { createSingle, PXScreen, graphInfo,
	PXView, PXFieldState, PXFieldOptions }
    from 'client-controls';

@graphInfo({graphType: 'PX.SM.WikiCssMaint', primaryView: 'WikiStyles'})
export class SM202030 extends PXScreen {
	WikiStyles = createSingle(WikiCss);
}

export class WikiCss extends PXView {
	Name: PXFieldState;
	Description: PXFieldState<PXFieldOptions.Multiline>;
	Style: PXFieldState<PXFieldOptions.Multiline>;
}