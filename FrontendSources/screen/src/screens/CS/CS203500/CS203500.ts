import { createCollection,createSingle,PXScreen,graphInfo,viewInfo } from 'client-controls';
import { UnitOfMeasure,INUnit } from './views';

@graphInfo({graphType: 'PX.Objects.Localizations.CA.CS.UnitOfMeasureMaint', primaryView: 'UnitOfMeasures', })
export class CS203500 extends PXScreen {


	@viewInfo({containerName: ''})
	UnitOfMeasures = createSingle(UnitOfMeasure);
	@viewInfo({containerName: ''})
	Units = createCollection(INUnit);

}
