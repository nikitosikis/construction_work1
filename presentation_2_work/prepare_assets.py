from pathlib import Path
from zipfile import ZipFile
from PIL import Image

work = Path(__file__).resolve().parent
archive = work.parent.parent / 'кудм_архив_2026-10-18'
ref = archive / 'Готовая_презентация' / 'ИТМО_Восстановление_Иванов_16_5–7_минут.pptx'
with ZipFile(ref) as package:
    (work / 'itmo_body.jpg').write_bytes(package.read('ppt/media/image17.jpg'))
im = Image.open(work / 'model.png')
im.crop((2280, 70, 3525, 1535)).save(work / 'journal_section.png')
for name in ['itmo_body.jpg', 'aeroC.png', 'journal_section.png']:
    with Image.open(work / name) as asset:
        print(name, asset.size)
