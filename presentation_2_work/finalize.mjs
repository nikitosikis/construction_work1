import path from 'node:path';
import fs from 'node:fs/promises';
import {createHash} from 'node:crypto';
import {fileURLToPath,pathToFileURL} from 'node:url';
const here=path.dirname(fileURLToPath(import.meta.url));
const workspaceDir=path.dirname(here);
const skill='C:/Users/jopajack/.codex/plugins/cache/openai-primary-runtime/presentations/26.1105.12137/skills/presentations';
const referencePath=path.join(path.dirname(workspaceDir),'кудм_архив_2026-10-18','Готовая_презентация','ИТМО_Восстановление_Иванов_16_5–7_минут.pptx');
const {finalizePresentation}=await import(pathToFileURL(path.join(skill,'container_tools','artifact_tool_utils.mjs')).href);
const tables=[4,8,9], charts=[7];
const result=await finalizePresentation({
  workspaceDir,
  candidatePath:path.join(here,'portable_candidate.pptx'),
  finalPath:path.join(workspaceDir,'Готовая_презентация','ИТМО_Доклад_2_Газовые_опоры_шпинделя_Иванов_16.pptx'),
  pythonExecutable:'C:/Users/jopajack/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe',
  integrityValidatorPath:path.join(skill,'container_tools','inspect_presentation_package_integrity.py'),
  layoutValidatorPath:path.join(skill,'container_tools','inspect_presentation_layout_geometry.py'),
  layoutArgs:['--expected-slide-size-emu','9144000,5143500','--validate-heading-fit',...tables.flatMap(n=>['--require-native-table-slide',String(n)])],
  explicitTotalSlideCount:13,
  requiredNativeTableOwnerSlides:tables,
  requiredNativeChartOwnerSlides:charts,
  materializeLiteralChartWorkbooks:true,
  fontPolicy:{basis:'reference',families:['Calibri','Golos Text','Golos Text DemiBold'],referencePath,referenceSha256:createHash('sha256').update(await fs.readFile(referencePath)).digest('hex')},
  verifyArtifactToolImport:true,
  receiptPath:path.join(here,'validation.json')
});
console.log(JSON.stringify({out:result.finalPath,integrity:result.packageIntegrity.status,findings:result.presentationLayout.findingCount,imports:result.firstPartyImport.passed}));
